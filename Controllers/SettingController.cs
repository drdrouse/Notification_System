using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DataHelper;
using DataAccessLibrary;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using DataAccessLibrary.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Notification_System.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly NotificationSystemContext _notificationSystemContext;
        public SettingController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }

        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            var phoneType = _notificationSystemContext.TypePhones.ToList();
            var mailType= _notificationSystemContext.TypeMails.ToList();
            ViewBag.phoneType = phoneType;
            ViewBag.mailType = mailType;
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

        [HttpPost]
        public async Task<IActionResult> AddPhone(string phoneType, string phoneNumber)
        {

            string result_error = "";
            string result_success = "Телефон успешно добавлен.";
            HttpContext.Session.Clear();
            if (!PhoneHelper.IsValidPhoneNumber(phoneNumber))
            {
                result_error += "Телефон введён в неверном формате.";
                HttpContext.Session.SetString("Phone", result_error);
                HttpContext.Session.SetString("PhoneMessage", "alert-error");
                HttpContext.Session.SetString("PhoneNum", "input-error");
            }
            else
            {
                if (AddNewPhone.AddPhone(phoneNumber, phoneType, Guid.Parse(User.Identity.Name)))
                {
                    HttpContext.Session.SetString("Phone", result_success);
                    HttpContext.Session.SetString("PhoneMessage", "alert-success");
                    HttpContext.Session.SetString("PhoneNum", "input-success");
                }
            }
            HttpContext.Session.SetString("OpenModalPhone", "true");
            return RedirectToAction("Index", "Setting");
        }

        [HttpPost]
        public async Task<IActionResult> AddMail(string mailType, string mail)
        {
            string result_error = "";
            string result_success = "Почта успешно добавлена.";
            HttpContext.Session.Clear();
            if (!MailHelper.IsValidEmail(mail))
            {
                result_error += "Почта введён в неверном формате.";
                HttpContext.Session.SetString("Mail", result_error);
                HttpContext.Session.SetString("MailMessage", "alert-error");
                HttpContext.Session.SetString("MailIn", "input-error");
            }
            else
            {
                if (AddNewMail.AddMail(mail, mailType, Guid.Parse(User.Identity.Name)))
                {
                    HttpContext.Session.SetString("Mail", result_success);
                    HttpContext.Session.SetString("MailMessage", "alert-success");
                    HttpContext.Session.SetString("MailIn", "input-success");
                }
            }
            HttpContext.Session.SetString("OpenModalMail", "true");
            return RedirectToAction("Index", "Setting");
        }
    }
}
