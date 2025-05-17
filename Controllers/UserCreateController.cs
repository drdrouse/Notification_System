using DataAccessLibrary;
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
            try
            {
                ViewData["ShowSideBarBlock"] = true;

                var profiles = _notificationSystemContext.Accounts
                    .Include(ac => ac.Profile).ThenInclude(p => p.Phones)
                    .Include(ac => ac.Profile).ThenInclude(p => p.Mail).ThenInclude(m => m.TypeMail)
                    .Include(ac => ac.RoleAssignments).ThenInclude(r => r.Role)
                    .Include(ac => ac.AccountStatus);
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "UserCreate_Open", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} has successfully opened a view");
                return View(profiles);
            }
            catch (Exception ex)
            {
                // Логирование ошибки (раскомментировать при необходимости)
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "UserCreate_Lost", ex.ToString());

                // В случае ошибки возвращаем пустой результат, сохраняя функциональность
                ViewData["ShowSideBarBlock"] = true;
                return View(Enumerable.Empty<Account>().AsQueryable());
            }
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "UserCreate"); // Редирект на главную страницу сервисов
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string Surname, string Name, string Patronymic)
        {
            try
            {
                string result_error = "";
                string result_success = "Пользователь успешно создан.";

                HttpContext.Session.Clear();

                // Проверка фамилии
                if (!DataHelper.UserHelper.CheckFLP(Surname))
                {
                    result_error += "Фамилия имеет неверный формат.\n";
                    HttpContext.Session.SetString("Message", result_error);
                    HttpContext.Session.SetString("MessageType", "alert-error");
                    HttpContext.Session.SetString("SurnameValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("SurnameValid", "input-success");

                // Проверка имени
                if (!DataHelper.UserHelper.CheckFLP(Name))
                {
                    result_error += "Имя имеет неверный формат.\n";
                    HttpContext.Session.SetString("Message", result_error);
                    HttpContext.Session.SetString("MessageType", "alert-error");
                    HttpContext.Session.SetString("NameValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("NameValid", "input-success");

                // Проверка отчества
                if (!DataHelper.UserHelper.CheckFLP(Patronymic))
                {
                    result_error += "Отчество имеет неверный формат.\n";
                    HttpContext.Session.SetString("Message", result_error);
                    HttpContext.Session.SetString("MessageType", "alert-error");
                    HttpContext.Session.SetString("PatronymicValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("PatronymicValid", "input-success");

                // Создание пользователя, если все данные валидны
                if (DataHelper.UserHelper.CheckFLP(Surname) &&
                    DataHelper.UserHelper.CheckFLP(Name) &&
                    DataHelper.UserHelper.CheckFLP(Patronymic))
                {
                    if (DataAccessLibrary.CreateUser.AddNewUser(Surname, Name, Patronymic) &&
                        DataAccessLibrary.CreateAccount.AddNewAccount(Surname, Name, Patronymic))
                    {
                        HttpContext.Session.SetString("Message", result_success);
                        HttpContext.Session.SetString("MessageType", "alert-success");
                        HttpContext.Session.SetString("SurnameValid", "input-success");
                        HttpContext.Session.SetString("NameValid", "input-success");
                        HttpContext.Session.SetString("PatronymicValid", "input-success");
                    }
                }
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_User_Success", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} has successfully added a new user");
                HttpContext.Session.SetString("OpenModal", "true");
                return RedirectToAction("Index", "UserCreate");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_User_Error", ex.ToString());

                // Очистка сессии и установка сообщения об ошибке
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("Message", "Произошла ошибка при создании пользователя");
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("OpenModal", "true");
                return RedirectToAction("Index", "UserCreate");
            }
        }
    }
}
