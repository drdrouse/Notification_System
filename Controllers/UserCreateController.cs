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
        public async Task<IActionResult> CreateUser(string Surname, string Name, string Patronymic)
        {

            string result_error = "";
            string result_success = "Пользователь успешно создан.";

            HttpContext.Session.Clear();

            if (!DataHelper.UserHelper.CheckFLP(Surname))
            {
                result_error += "Ошибка вводе фамилии\n";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("SurnameValid", "input-error");
            }
            else
                HttpContext.Session.SetString("SurnameValid", "input-success");

            if (!DataHelper.UserHelper.CheckFLP(Name))
            {
                result_error += "Ошибка вводе имени\n";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("NameValid", "input-error");
            }
            else
                HttpContext.Session.SetString("NameValid", "input-success");

            if (!DataHelper.UserHelper.CheckFLP(Patronymic))
            {
                result_error += "Ошибка вводе имени\n";
                HttpContext.Session.SetString("Message", result_error);
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("PatronymicValid", "input-error");
            }
            else
                HttpContext.Session.SetString("PatronymicValid", "input-success");

            HttpContext.Session.SetString("OpenModal", "true");
            return RedirectToAction("Index", "UserCreate");
        }
    }
}
