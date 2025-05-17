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
        public IActionResult Index(string login, string password)
        {
            if (AuthorisationConfirm.CheckStatus(login))
            {
                if (AuthorisationConfirm.LoginPasswordAccept(login, password))
                {

                    claims.Add(new Claim(ClaimTypes.Name, AuthorisationConfirm.AccountID().ToString()));

                    var roles = AuthorisationConfirm.Role(AuthorisationConfirm.AccountID());
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                    }
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // Запомнить пользователя между сессиями
                    };

                    HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                    
                    string description = $"User {Log_Creater.TabNum(AuthorisationConfirm.AccountID())} logged into the account"; 
                    Log_Creater.Create(AuthorisationConfirm.AccountID(), "LogOn", description);

                    return RedirectToAction("Index", "Account");
                }
                ViewBag.Error = "Неверно введены данные";
                return View();
            }

            ViewBag.Error = "Ваш аккаунт заблокирован. Обратитесь к администратору.";
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Exite()
        {
            // Удаляем куки авторизации
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            string description = $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} logged out of the account";
            Log_Creater.Create(Guid.Parse(User.Identity.Name), "LogOff", description);
            return RedirectToAction("Index", "Authorisation");
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
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
    }
}
