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
            if (!Change_Password.ConfirmOldPassword(hash_oldPassword, Guid.Parse(User.Identity.Name)))
            {
                HttpContext.Session.SetString("Message", "Неверно введён текущий пароль.");
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("OldPasswordValid", "input-error");
            }

            //if (newPassword != confirmPassword)
            //{
            //    HttpContext.Session.SetString("Message", "Новый пароль и подтверждение не совпадают.");
            //    HttpContext.Session.SetString("MessageType", "alert-error");
            //    HttpContext.Session.SetString("OldPasswordValid", "input-success");
            //    HttpContext.Session.SetString("NewPasswordValid", "input-error");
            //    HttpContext.Session.SetString("ConfirmPasswordValid", "input-error");
            //}
            //else
            //{
            //    HttpContext.Session.SetString("Message", "Пароль успешно изменён!");
            //    HttpContext.Session.SetString("MessageType", "alert-success");
            //    HttpContext.Session.SetString("OldPasswordValid", "input-success");
            //    HttpContext.Session.SetString("NewPasswordValid", "input-success");
            //    HttpContext.Session.SetString("ConfirmPasswordValid", "input-success");
            //}

            HttpContext.Session.SetString("OpenModal", "true");
            return RedirectToAction("Index", "Setting");
        }
    }
}
