using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccessLibrary;
using Microsoft.Identity.Client;
using DataAccessLibrary.Models;

namespace Notification_System.Controllers
{
    [AllowAnonymous]
    public class AuthorisationController : Controller
    {
        private NotificationSystemContext _notificationSystemContext;
        List<Claim> claims = new List<Claim>();
        [HttpPost]
        public async Task<IActionResult> Index(string login, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Неверно введены данные";
                    return View();
                }

                if (!AuthorisationConfirm.CheckStatus(login))
                {
                    ViewBag.Error = "Ваш аккаунт заблокирован. Обратитесь к администратору.";
                    return View();
                }

                if (!AuthorisationConfirm.LoginPasswordAccept(login, password))
                {
                    ViewBag.Error = "Неверно введены данные";
                    return View();
                }

                var accountId = AuthorisationConfirm.AccountID();
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, accountId.ToString())
        };

                foreach (var role in AuthorisationConfirm.Role(accountId))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                string description = $"User {Log_Creater.TabNum(accountId)} logged into the account";
                Log_Creater.Create(accountId, "LogOn", description);

                return RedirectToAction("Index", "Account");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "LogOn", ex.ToString());

                ViewBag.Error = "Произошла ошибка при авторизации. Пожалуйста, попробуйте позже.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Exite()
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    // Если пользователь не аутентифицирован, просто перенаправляем
                    return RedirectToAction("Index", "Authorisation");
                }

                // Удаляем куки авторизации
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                Guid userId = Guid.Parse(User.Identity.Name);
                string description = $"User {Log_Creater.TabNum(userId)} logged out of the account";

                Log_Creater.Create(userId, "LogOff", description);

                return RedirectToAction("Index", "Authorisation");
            }
            catch (Exception ex)
            {
                // Логирование ошибки (можно добавить ваш логгер)
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "LogOff", ex.ToString());

                // В случае ошибки все равно перенаправляем на страницу авторизации
                return RedirectToAction("Index", "Authorisation");
            }
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    // Если пользователь уже авторизован, перенаправляем его на главную страницу
                    return RedirectToAction("Index", "Account");
                }

                ViewData["ReturnUrl"] = returnUrl;
                ViewData["ShowSideBarBlock"] = false;
                return View();
            }
            catch (Exception ex)
            {
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "LogOn", ex.ToString());

                // В случае ошибки сохраняем минимальную функциональность
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["ShowSideBarBlock"] = false;
                return View();
            }
        }
    }
}
