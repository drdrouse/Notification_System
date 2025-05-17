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
            try
            {
                ViewData["ShowSideBarBlock"] = true;

                // Получаем список типов телефонов
                var phoneType = _notificationSystemContext.TypePhones
                    .AsNoTracking()  // Оптимизация - только для чтения
                    .ToList();

                // Получаем список типов почты
                var mailType = _notificationSystemContext.TypeMails
                    .AsNoTracking()  // Оптимизация - только для чтения
                    .ToList();

                ViewBag.phoneType = phoneType ?? new List<TypePhone>();  // Защита от null
                ViewBag.mailType = mailType ?? new List<TypeMail>();    // Защита от null
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Setting_Open", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} opened setting view");
                return View();
            }
            catch (Exception ex)
            {
                // Логирование ошибки (раскомментировать при необходимости)
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Setting_Open", ex.ToString());

                // Возвращаем корректное состояние даже при ошибке
                ViewData["ShowSideBarBlock"] = true;
                ViewBag.phoneType = new List<TypePhone>();
                ViewBag.mailType = new List<TypeMail>();
                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            try
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
                    string description = $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} changed their password";
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Change_Password", description);
                }

                HttpContext.Session.SetString("OpenModal", "true");
                return RedirectToAction("Index", "Setting");
            }
            catch (Exception)
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("Message", "Произошла ошибка при изменении пароля");
                HttpContext.Session.SetString("MessageType", "alert-error");
                HttpContext.Session.SetString("OpenModal", "true");
                return RedirectToAction("Index", "Setting");
            }
        }


        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "Setting"); // Редирект на главную страницу настроек
        }

        [HttpPost]
        public async Task<IActionResult> AddPhone(string phoneType, string phoneNumber)
        {
            try
            {
                const string resultError = "Телефон введён в неверном формате.";
                const string resultSuccess = "Телефон успешно добавлен.";

                HttpContext.Session.Clear();

                // Проверка валидности номера телефона
                if (!PhoneHelper.IsValidPhoneNumber(phoneNumber))
                {
                    SetPhoneSession(HttpContext, resultError, "alert-error", "input-error");
                }
                else
                {
                    // Попытка добавления телефона
                    bool addResult = false;
                    try
                    {
                        addResult = AddNewPhone.AddPhone(
                            phoneNumber,
                            phoneType,
                            Guid.Parse(User.Identity.Name));
                    }
                    catch (Exception ex)
                    {
                        // Логирование ошибки добавления
                        Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_Phone_Error", ex.ToString());
                        SetPhoneSession(HttpContext, "Ошибка при добавлении телефона", "alert-error", "input-error");
                        return RedirectToAction("Index", "Setting");
                    }

                    if (addResult)
                    {
                        Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_Phone_Success", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} has successfully added a phone");
                        SetPhoneSession(HttpContext, resultSuccess, "alert-success", "input-success");
                    }
                    else
                    {
                        SetPhoneSession(HttpContext, "Не удалось добавить телефон", "alert-error", "input-error");
                    }
                }

                HttpContext.Session.SetString("OpenModalPhone", "true");
                return RedirectToAction("Index", "Setting");
            }
            catch (Exception ex)
            {
                // Логирование общей ошибки
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_Phone_Error", ex.ToString());

                // Минимальная обработка для сохранения UX
                HttpContext.Session.Clear();
                SetPhoneSession(HttpContext, "Произошла ошибка", "alert-error", "input-error");
                HttpContext.Session.SetString("OpenModalPhone", "true");
                return RedirectToAction("Index", "Setting");
            }
        }

        // Вспомогательный метод для установки значений сессии
        private void SetPhoneSession(HttpContext httpContext, string message, string alertType, string inputType)
        {
            httpContext.Session.SetString("Phone", message);
            httpContext.Session.SetString("PhoneMessage", alertType);
            httpContext.Session.SetString("PhoneNum", inputType);
        }

        [HttpPost]
        public async Task<IActionResult> AddMail(string mailType, string mail)
        {
            try
            {
                const string resultError = "Почта введена в неверном формате.";
                const string resultSuccess = "Почта успешно добавлена.";

                // Очищаем сессию перед началом операции
                HttpContext.Session.Clear();

                // Проверка валидности email
                if (!MailHelper.IsValidEmail(mail))
                {
                    SetMailSession(HttpContext, resultError, "alert-error", "input-error");
                }
                else
                {
                    // Попытка добавления почты
                    bool addResult = false;
                    try
                    {
                        addResult = AddNewMail.AddMail(
                            mail,
                            mailType,
                            Guid.Parse(User.Identity.Name));
                    }
                    catch (FormatException)
                    {
                        SetMailSession(HttpContext, "Ошибка формата данных", "alert-error", "input-error");
                    }
                    catch (Exception ex)
                    {
                        // Логирование ошибки
                        Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_Email_Error", ex.ToString());
                        SetMailSession(HttpContext, "Ошибка при добавлении почты", "alert-error", "input-error");
                        return RedirectToAction("Index", "Setting");
                    }

                    if (addResult)
                    {
                        Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_Email_Success", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} has successfully added a email");
                        SetMailSession(HttpContext, resultSuccess, "alert-success", "input-success");
                    }
                    else
                    {
                        SetMailSession(HttpContext, "Не удалось добавить почту", "alert-error", "input-error");
                    }
                }

                // Открываем модальное окно после операции
                HttpContext.Session.SetString("OpenModalMail", "true");
                return RedirectToAction("Index", "Setting");
            }
            catch (Exception ex)
            {
                // Логирование критической ошибки
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_Email_Error", ex.ToString());

                // Минимальная обработка ошибки для сохранения UX
                HttpContext.Session.Clear();
                SetMailSession(HttpContext, "Произошла ошибка", "alert-error", "input-error");
                HttpContext.Session.SetString("OpenModalMail", "true");
                return RedirectToAction("Index", "Setting");
            }
        }

        // Вспомогательный метод для установки значений сессии
        private void SetMailSession(HttpContext httpContext, string message, string alertType, string inputType)
        {
            httpContext.Session.SetString("Mail", message);
            httpContext.Session.SetString("MailMessage", alertType);
            httpContext.Session.SetString("MailIn", inputType);
        }
    }
}
