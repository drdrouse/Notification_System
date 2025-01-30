using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DataHelper;
using DataAccessLibrary;

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
            string hash_confirmPassword = PasswordHelper.SHA256Convert(confirmPassword);
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
                result_error += "Пароли не совпадают.";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("NewPasswordValid", "input-error");
                HttpContext.Session.SetString("ConfirmPasswordValid", "input-error");
            }
            
            if (Change_Password.ConfirmOldPassword(hash_oldPassword, Guid.Parse(User.Identity.Name)) &&
                PasswordHelper.CheckPassword(newPassword) &&
                PasswordHelper.PasswordComparison(newPassword, confirmPassword))
            {
                HttpContext.Session.SetString("Message", "Пароль успешно изменён!");
                HttpContext.Session.SetString("MessageType", "alert-success");
                HttpContext.Session.SetString("OldPasswordValid", "input-success");
                HttpContext.Session.SetString("NewPasswordValid", "input-success");
                HttpContext.Session.SetString("ConfirmPasswordValid", "input-success");
            }

            HttpContext.Session.SetString("OpenModal", "true");
            return RedirectToAction("Index", "Setting");
        }
    }
}
