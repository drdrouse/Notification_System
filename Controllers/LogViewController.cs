using DataAccessLibrary;
using DataAccessLibrary.Models;
using DataHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Notification_System.Controllers
{
    public class LogViewController : Controller
    {
        private readonly NotificationSystemContext _notificationSystemContext;

        public LogViewController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }
        public IActionResult Index()
        {
            try
            {
                ViewData["ShowSideBarBlock"] = true;

                var log = _notificationSystemContext.Logs
                    .Include(p => p.Profile)
                    .Include(e => e.EventCode)
                    .OrderByDescending(l => l.LogDateTime) // предполагаем, что есть свойство Date
                    .ToList();
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Log_Open", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} opened the logs tab");
                return View(log);
            }
            catch (Exception ex)
            {
                // Логирование ошибки (можно добавить ваш логгер)
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Log_Lost", ex.ToString());

                // В случае ошибки возвращаем пустой список и сохраняем функциональность
                ViewData["ShowSideBarBlock"] = true;
                return View(new List<Log>());
            }
        }

        [HttpPost]
        public IActionResult UserFilter(string selectedUser)
        {
            HttpContext.Session.Clear();

            if (string.IsNullOrEmpty(selectedUser) || selectedUser == "-- Все пользователи --")
            {
                HttpContext.Session.SetString("UserMessageType", "alert-error");
                HttpContext.Session.SetString("UserMessage", "Чтобы применть фильтр, необходимо выбрать значение");
            }
            else
            {
                HttpContext.Session.SetString("UserMessageType", "alert-success");
                HttpContext.Session.SetString("UserMessage", "Фильтр по пользователям успешно применён");
            }

           HttpContext.Session.SetString("OpenModalUser", "true");
            return RedirectToAction("Index", "LogView");
        }

        [HttpPost]
        public IActionResult DataFilter(string startDate, string endDate)
        {
            HttpContext.Session.Clear();
            string result_error = "";

            if (string.IsNullOrEmpty(startDate))
            {
                result_error += "Поле для ввода начальной даты не может быть пустым\n";
                HttpContext.Session.SetString("StrartDate", "input-error");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                result_error += "Поле для ввода конечной даты не может быть пустым";
                HttpContext.Session.SetString("EndDate", "input-error");
            }
            if (!string.IsNullOrEmpty(startDate) && !LogHelper.IsDateInCorrectFormat(startDate))
            {
                result_error += "Начальная дата имеет неверный формат\n";
                HttpContext.Session.SetString("StrartDate", "input-error");
            }
            if (!string.IsNullOrEmpty(endDate) && !LogHelper.IsDateInCorrectFormat(endDate))
            {
                result_error += "Конечная дата имеет неверный формат\n";
                HttpContext.Session.SetString("EndDate", "input-error");
            }
            if (!string.IsNullOrEmpty(result_error))
            {
                HttpContext.Session.SetString("DateMessage", result_error);
                HttpContext.Session.SetString("DateMessageType", "alert-error");
                HttpContext.Session.SetString("OpenModalData", "true");
                return RedirectToAction("Index", "LogView");
            }
            else
            {
                HttpContext.Session.SetString("DateMessage", "Фильтр по дате применён");
                HttpContext.Session.SetString("DateMessageType", "alert-success");
            }

            HttpContext.Session.SetString("OpenModalData", "true");
            return RedirectToAction("Index", "LogView");
        }

        [HttpPost]
        public IActionResult ActionFilter(string selectedAction)
        {

            HttpContext.Session.Clear();

            if (string.IsNullOrEmpty(selectedAction) || selectedAction == "-- Все события --")
            {
                HttpContext.Session.SetString("ActionMessageType", "alert-error");
                HttpContext.Session.SetString("ActionMessage", "Чтобы применть фильтр, необходимо выбрать значение");
            }
            else
            {
                HttpContext.Session.SetString("ActionMessageType", "alert-success");
                HttpContext.Session.SetString("ActionMessage", "Фильтр по событиям успешно применён");
            }
            HttpContext.Session.SetString("OpenModalAction", "true");
            return RedirectToAction("Index", "LogView");
        }


        //[HttpPost]
        //public IActionResult ClearFilter()
        //{
        //    // Обработка выбранного значения
        //    // ...
        //}

        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "LogView"); // Редирект на главную страницу настроек
        }
    }
}
