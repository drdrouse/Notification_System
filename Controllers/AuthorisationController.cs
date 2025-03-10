﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccessLibrary;
using Microsoft.Identity.Client;

namespace Notification_System.Controllers
{
    [AllowAnonymous]
    public class AuthorisationController : Controller
    {
        [HttpPost]
        public IActionResult Index(string login, string password)
        {
            if (AuthorisationConfirm.LoginPasswordAccept(login, password))
            {

                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, AuthorisationConfirm.AccountID().ToString()),
                new Claim(ClaimTypes.Role, AuthorisationConfirm.Role(AuthorisationConfirm.AccountID())) 
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Запомнить пользователя между сессиями
                };

                HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

                Log_Creater.Create(AuthorisationConfirm.AccountID(), "Enter");

                return RedirectToAction("Index", "Account");
            }

            ViewBag.Error = "Неверно введены данные";
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Exite()
        {
            // Удаляем куки авторизации
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Log_Creater.Create(Guid.Parse(User.Identity.Name), "Exite");
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
