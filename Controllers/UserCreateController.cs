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
                var posts = _notificationSystemContext.Posts.ToList();

                var profiles = _notificationSystemContext.Accounts
                    .Include(ac => ac.Profile).ThenInclude(p => p.Phones)
                    .Include(ac => ac.Profile).ThenInclude(p => p.Mail).ThenInclude(m => m.TypeMail)
                    .Include(ac => ac.RoleAssignments).ThenInclude(r => r.Role)
                    .Include(ac => ac.AccountStatus)
                    .Include(ac => ac.Profile).ThenInclude(or => or.OrganizationUnits).ThenInclude(po => po.Post);
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "UserCreate_Open", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} has successfully opened a view");
                
                ViewBag.Posts = posts;
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
        public async Task<IActionResult> CreateUser(
            string Surname,
            string Name,
            string Patronymic,
            string CorporatePhone,
            string CorporateEmail,
            string EmployeeNumber,
            string Position,
            string Role)
        {
            try
            {
                string result_error = "";
                string result_success = "Пользователь успешно создан.";

                HttpContext.Session.Clear();

                // Проверка персональных данных
                if (!DataHelper.UserHelper.CheckFLP(Surname))
                {
                    result_error += "Фамилия имеет неверный формат.\n";
                    HttpContext.Session.SetString("SurnameValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("SurnameValid", "input-success");

                if (!DataHelper.UserHelper.CheckFLP(Name))
                {
                    result_error += "Имя имеет неверный формат.\n";
                    HttpContext.Session.SetString("NameValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("NameValid", "input-success");

                if (!DataHelper.UserHelper.CheckFLP(Patronymic))
                {
                    result_error += "Отчество имеет неверный формат.\n";
                    HttpContext.Session.SetString("PatronymicValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("PatronymicValid", "input-success");

                // Проверка корпоративных данных
                if (!string.IsNullOrEmpty(CorporatePhone) && !DataHelper.UserHelper.CheckPhoneFormat(CorporatePhone))
                {
                    result_error += "Неверный формат корпоративного телефона.\n";
                    HttpContext.Session.SetString("CorporatePhoneValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("CorporatePhoneValid", "input-success");

                if (!string.IsNullOrEmpty(CorporateEmail) && !DataHelper.UserHelper.CheckEmailFormat(CorporateEmail))
                {
                    result_error += "Неверный формат корпоративной почты.\n";
                    HttpContext.Session.SetString("CorporateEmailValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("CorporateEmailValid", "input-success");

                if (!DataHelper.UserHelper.CheckEmployeeNumber(EmployeeNumber))
                {
                    result_error += "Табельный номер должен состоять из 7 цифр.\n";
                    HttpContext.Session.SetString("EmployeeNumberValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("EmployeeNumberValid", "input-success");

                if (string.IsNullOrWhiteSpace(Position) || Position == "-- Выберите должность --")
                {
                    result_error += "Необходимо выбрать должность.\n";
                    HttpContext.Session.SetString("PositionValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("PositionValid", "input-success");

                if (string.IsNullOrWhiteSpace(Role) || Role == "-- Выберите роль --")
                {
                    result_error += "Необходимо выбрать роль пользователя.\n";
                    HttpContext.Session.SetString("RoleValid", "input-error");
                }
                else
                    HttpContext.Session.SetString("RoleValid", "input-success");

                // Если есть ошибки - сохраняем их и возвращаем
                if (!string.IsNullOrEmpty(result_error))
                {
                    HttpContext.Session.SetString("Message", result_error);
                    HttpContext.Session.SetString("MessageType", "alert-error");
                    HttpContext.Session.SetString("OpenModal", "true");
                    return RedirectToAction("Index", "UserCreate");
                }

                // Создание пользователя, если все данные валидны
                if (DataAccessLibrary.CreateUser.AddNewUser(
                    Surname,
                    Name,
                    Patronymic,
                    CorporatePhone,
                    CorporateEmail,
                    EmployeeNumber,
                    Position) &&
                    CreateAccount.AddNewAccount(
                        Surname,
                        Name,
                        Patronymic,
                        Role,
                        EmployeeNumber))
                {
                    HttpContext.Session.SetString("Message", result_success);
                    HttpContext.Session.SetString("MessageType", "alert-success");
                    // Сбрасываем все валидационные статусы в success
                    HttpContext.Session.SetString("SurnameValid", "input-success");
                    HttpContext.Session.SetString("NameValid", "input-success");
                    HttpContext.Session.SetString("PatronymicValid", "input-success");
                    HttpContext.Session.SetString("CorporatePhoneValid", "input-success");
                    HttpContext.Session.SetString("CorporateEmailValid", "input-success");
                    HttpContext.Session.SetString("EmployeeNumberValid", "input-success");
                    HttpContext.Session.SetString("PositionValid", "input-success");
                    HttpContext.Session.SetString("RoleValid", "input-success");
                }

                Log_Creater.Create(Guid.Parse(User.Identity.Name),
                    "Add_User_Success",
                    $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} has successfully added a new user: {EmployeeNumber}");

                HttpContext.Session.SetString("OpenModal", "true");
                return RedirectToAction("Index", "UserCreate");
            }
            catch (Exception ex)
            {
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_User_Error", ex.ToString());
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("Message", "Произошла ошибка при создании пользователя");
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("OpenModal", "true");
                return RedirectToAction("Index", "UserCreate");
            }
        }
    }
}
