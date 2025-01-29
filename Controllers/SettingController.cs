using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["Message"] = "Все поля обязательны для заполнения.";
                TempData["MessageType"] = "alert-error";
                TempData["OldPasswordValid"] = string.IsNullOrEmpty(oldPassword) ? "input-error" : "input-success";
                TempData["NewPasswordValid"] = string.IsNullOrEmpty(newPassword) ? "input-error" : "input-success";
                TempData["ConfirmPasswordValid"] = string.IsNullOrEmpty(confirmPassword) ? "input-error" : "input-success";
            }
            else if (newPassword != confirmPassword)
            {
                TempData["Message"] = "Новый пароль и подтверждение не совпадают.";
                TempData["MessageType"] = "alert-error";
                TempData["OldPasswordValid"] = "input-success";
                TempData["NewPasswordValid"] = "input-error";
                TempData["ConfirmPasswordValid"] = "input-error";
            }
            else
            {
                // Логика смены пароля
                TempData["Message"] = "Пароль успешно изменён!";
                TempData["MessageType"] = "alert-success";
                TempData["OldPasswordValid"] = "input-success";
                TempData["NewPasswordValid"] = "input-success";
                TempData["ConfirmPasswordValid"] = "input-success";
            }

            TempData["OpenModal"] = true;
            return RedirectToAction("Index", "Setting");
        }
    }
}
