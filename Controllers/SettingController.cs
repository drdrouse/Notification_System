﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DataHelper;
using DataAccessLibrary;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Notification_System.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            string hash_oldPassword = PasswordHelper.SHA256Convert(oldPassword);
            string hash_newPassword = PasswordHelper.SHA256Convert(newPassword);
            string result_error = "";
            string result_success = "Пароль успешно изменён.";

            HttpContext.Session.Clear();

            if (!Change_Password.ConfirmOldPassword(hash_oldPassword, Guid.Parse(User.Identity.Name)))
            {
                result_error += "Неверно введён текущий пароль.\n";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("OldPasswordValid", "input-error");
            }
            else
                HttpContext.Session.SetString("OldPasswordValid", "input-success");


            if (!PasswordHelper.CheckPassword(newPassword))
            {
                result_error += "Новый пароль не соответствует требованиям.\n";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("NewPasswordValid", "input-error");
                HttpContext.Session.SetString("ConfirmPasswordValid", "input-error");
            }
            
            if (!PasswordHelper.PasswordComparison(newPassword, confirmPassword))
            {
                result_error += "Пароли не совпадают.\n";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("NewPasswordValid", "input-error");
                HttpContext.Session.SetString("ConfirmPasswordValid", "input-error");
            }

            if (!Change_Password.CheckPasswordRepeat(hash_newPassword, Guid.Parse(User.Identity.Name)))
            {
                result_error += "Данный пароль уже использовался раньше.";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("NewPasswordValid", "input-error");
                HttpContext.Session.SetString("ConfirmPasswordValid", "input-error");
            }
            
            if (Change_Password.ConfirmOldPassword(hash_oldPassword, Guid.Parse(User.Identity.Name)) &&
                PasswordHelper.CheckPassword(newPassword) &&
                PasswordHelper.PasswordComparison(newPassword, confirmPassword) &&
                Change_Password.CheckPasswordRepeat(hash_newPassword, Guid.Parse(User.Identity.Name)))
            {
                Change_Password.CancelOldPassword(hash_oldPassword, Guid.Parse(User.Identity.Name));
                Change_Password.SaveNewPassword(hash_newPassword, Guid.Parse(User.Identity.Name));
                HttpContext.Session.SetString("Message", "Пароль успешно изменён!");
                HttpContext.Session.SetString("MessageType", "alert-success");
                HttpContext.Session.SetString("OldPasswordValid", "input-success");
                HttpContext.Session.SetString("NewPasswordValid", "input-success");
                HttpContext.Session.SetString("ConfirmPasswordValid", "input-success");
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Change_Password");
            }

            HttpContext.Session.SetString("OpenModal", "true");
            return RedirectToAction("Index", "Setting");
        }


        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "Setting"); // Редирект на главную страницу настроек
        }
    }
}
