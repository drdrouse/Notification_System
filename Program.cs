using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка аутентификации с безопасными параметрами
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authorisation/Index";
        options.AccessDeniedPath = "/Authorisation/Index";
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Общее время жизни куки
        options.SlidingExpiration = true; // Обновлять куки при активности пользователя

        // Дополнительные настройки безопасности
        options.Cookie = new CookieBuilder
        {
            HttpOnly = true, // Защита от XSS
            SecurePolicy = CookieSecurePolicy.Always, // Только HTTPS (если есть)
            SameSite = SameSiteMode.Strict, // Защита от CSRF
            Name = "AuthCookie" // Явное имя куки
        };
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Важно: UseAuthentication ДО UseAuthorization
app.UseAuthentication(); // Добавлено (раньше пропущено!)
app.UseSession();
app.UseAuthorization();

// Middleware для проверки истекшей аутентификации
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var authExpireClaim = context.User.FindFirstValue(ClaimTypes.Expired);
        if (authExpireClaim != null && DateTime.Parse(authExpireClaim) < DateTime.UtcNow)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Response.Redirect("/Authorisation/Index");
            return;
        }
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authorisation}/{action=Index}/{id?}");

app.Run();