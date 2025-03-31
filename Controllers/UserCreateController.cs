using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Notification_System.Controllers
{


    public class UserCreateController : Controller
    {

        private readonly NotificationSystemContext _notificationSystemContext;

        public UserCreateController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }


        [Authorize(Roles = "UserCreator,Admin")]
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            var profiles = _notificationSystemContext.Accounts.
                Include(ac => ac.Profile).ThenInclude(p => p.Phones).
                Include(ac => ac.Profile).ThenInclude(p => p.Mail).ThenInclude(m => m.TypeMail).
                Include(ac => ac.RoleAssignments).ThenInclude(r => r.Role).
                Include(ac => ac.AccountStatus);
            return View(profiles);
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "UserCreate"); // Редирект на главную страницу сервисов
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string Surname)
        {

            string result_error = "";
            string result_success = "Пользователь успешно создан";

            HttpContext.Session.Clear();

            if (Surname == "Иванов")
            {
                result_error += "Неверно введёна фамилия\n";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("SurnameValid", "input-error");
            }
            else
                HttpContext.Session.SetString("SurnameValid", "input-success");

            HttpContext.Session.SetString("OpenModal", "true");
            return RedirectToAction("Index", "UserCreate");
        }
    }
}
