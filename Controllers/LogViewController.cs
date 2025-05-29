using DataAccessLibrary;
using DataAccessLibrary.Models;
using DataHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Globalization;

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

            // Получаем базовый запрос
            IQueryable<Log> query = _notificationSystemContext.Logs
                .Include(p => p.Profile)
                .Include(e => e.EventCode)
                .OrderByDescending(l => l.LogDateTime);

            // Применяем фильтр по пользователю (если установлен)
            string userFilter = HttpContext.Session.GetString("UserFilter");
            if (!string.IsNullOrEmpty(userFilter))
            {
                query = query.Where(l => l.Profile.ProfileTabNum == int.Parse(userFilter));
            }

            // Применяем фильтр по дате (если установлен)
            string startDateStr = HttpContext.Session.GetString("StartDateFilter");
            string endDateStr = HttpContext.Session.GetString("EndDateFilter");
        
            if (DateTime.TryParseExact(startDateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                query = query.Where(l => l.LogDateTime >= startDate);
            }
        
            if (DateTime.TryParseExact(endDateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                query = query.Where(l => l.LogDateTime <= endDate.AddDays(1)); // Добавляем день для включения всей конечной даты
            }

            // Применяем фильтр по событию (если установлен)
            string actionFilter = HttpContext.Session.GetString("ActionFilter");
            if (!string.IsNullOrEmpty(actionFilter))
            {
                query = query.Where(l => l.EventCode.EventCodeName == actionFilter);
            }

            var log = query.ToList();
            Log_Creater.Create(Guid.Parse(User.Identity.Name), "Log_Open", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} opened the logs tab");
        
            return View(log);
            }
            catch (Exception ex)
            {
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Log_Lost", ex.ToString());
                ViewData["ShowSideBarBlock"] = true;
                return View(new List<Log>());
            }
        }

        [HttpPost]
        [HttpPost]
        public IActionResult UserFilter(string selectedUser)
        {
            try
            {
                // Очищаем только связанные с этим фильтром сессии
                HttpContext.Session.Remove("UserMessageType");
                HttpContext.Session.Remove("UserMessage");
                HttpContext.Session.Remove("UserFilter");

                if (string.IsNullOrEmpty(selectedUser) || selectedUser == "-- Все пользователи --")
                {
                    HttpContext.Session.SetString("UserMessageType", "alert-error");
                    HttpContext.Session.SetString("UserMessage", "Для применения фильтра должно быть выбрано значение");
                }
                else
                {
                    HttpContext.Session.SetString("UserMessageType", "alert-success");
                    HttpContext.Session.SetString("UserMessage", "Фильтр по пользователям успешно применён");
                    HttpContext.Session.SetString("UserFilter", selectedUser);
                }

                HttpContext.Session.SetString("OpenModalUser", "true");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Filter_Error",
                    $"Error UserFilter: {ex.Message}\nParameter: {selectedUser}\n{ex.StackTrace}");

                // Устанавливаем сообщение об ошибке для пользователя
                HttpContext.Session.SetString("UserMessageType", "alert-danger");
                HttpContext.Session.SetString("UserMessage", "Произошла ошибка при применении фильтра");

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [HttpPost]
        public IActionResult DataFilter(string startDate, string endDate)
        {
            try
            {
                // Очищаем только связанные с этим фильтром сессии
                HttpContext.Session.Remove("DateMessage");
                HttpContext.Session.Remove("DateMessageType");
                HttpContext.Session.Remove("StartDateFilter");
                HttpContext.Session.Remove("EndDateFilter");
                HttpContext.Session.Remove("StrartDate");
                HttpContext.Session.Remove("EndDate");

                string result_error = "";
                if (string.IsNullOrEmpty(startDate))
                {
                    result_error += "Начальная дата имеет неверный формат\n";
                    HttpContext.Session.SetString("StrartDate", "input-error"); ;
                }

                if (string.IsNullOrEmpty(endDate))
                {
                    result_error += "Конечная дата имеет неверный формат\n";
                    HttpContext.Session.SetString("EndDate", "input-error");
                }

                // Валидация дат
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
                    return RedirectToAction("Index");
                }
                else
                {
                    HttpContext.Session.SetString("DateMessage", "Фильтр по дате применён");
                    HttpContext.Session.SetString("DateMessageType", "alert-success");
                    HttpContext.Session.SetString("OpenModalData", "true");
                }

                // Сохраняем фильтры
                if (!string.IsNullOrEmpty(startDate))
                {
                    HttpContext.Session.SetString("StartDateFilter", startDate);
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    HttpContext.Session.SetString("EndDateFilter", endDate);
                }

                

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Filter_Error",
                    $"Error в DataFilter: {ex.Message}\nParameter: startDate={startDate}, endDate={endDate}\n{ex.StackTrace}");

                // Устанавливаем сообщение об ошибке для пользователя
                HttpContext.Session.SetString("DateMessageType", "alert-danger");
                HttpContext.Session.SetString("DateMessage", "Произошла ошибка при обработке фильтра по дате");
                HttpContext.Session.SetString("OpenModalData", "true");

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult ActionFilter(string selectedAction)
        {
            // Очищаем только связанные с этим фильтром сессии
            HttpContext.Session.Remove("ActionMessageType");
            HttpContext.Session.Remove("ActionMessage");
            HttpContext.Session.Remove("ActionFilter");

            if (string.IsNullOrEmpty(selectedAction) || selectedAction == "-- Все события --")
            {
                HttpContext.Session.SetString("ActionMessageType", "alert-error");
                HttpContext.Session.SetString("ActionMessage", "Для применения фильтра должно быть выбрано значение");
            }
            else
            {
                HttpContext.Session.SetString("ActionMessageType", "alert-success");
                HttpContext.Session.SetString("ActionMessage", "Фильтр по событиям успешно применён");
                HttpContext.Session.SetString("ActionFilter", selectedAction);
            }
            Log_Creater.Create(Guid.Parse(User.Identity.Name), "Filter_Error",
                    $"Error в DataFilter: {ex.Message}\nParameter: startDate={startDate}, endDate={endDate}\n{ex.StackTrace}");
            HttpContext.Session.SetString("OpenModalAction", "true");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateReport(string reportFormat)
        {
            try
            {
                if (string.IsNullOrEmpty(reportFormat) || reportFormat== "-- Выберите формат --")
                {
                    HttpContext.Session.SetString("ReportMessageType", "alert-error");
                    HttpContext.Session.SetString("ReportMessage", "Необходимо выбрать формат отчёта");
                }
                else
                {
                    // Логика генерации отчёта в выбранном формате
                    switch (reportFormat.ToUpper())
                    {
                        case "XML":
                            // Генерация XML
                            break;
                        case "JSON":
                            // Генерация JSON
                            break;
                        case "CSV":
                            // Генерация CSV
                            break;
                        case "PARQUET":
                            // Генерация Parquet
                            break;
                        default:
                            throw new ArgumentException("Неподдерживаемый формат отчёта");
                    }
                }

                HttpContext.Session.SetString("ReportMessageType", "alert-success");
                HttpContext.Session.SetString("ReportMessage", $"Отчёт в формате {reportFormat} успешно сформирован");
            }
            catch (Exception ex)
            {
                Log_Creater.Create(
                    Guid.Parse(User.Identity.Name),
                    "Error_Report",
                    $"Error: {ex.ToString()}.\nWhen creating a report in report format: {reportFormat} "
                );
                HttpContext.Session.SetString("ReportMessageType", "alert-error");
                HttpContext.Session.SetString("ReportMessage", $"Ошибка при формировании отчёта: {ex.Message}");
            }

            HttpContext.Session.SetString("OpenReportForm", "true");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearFilters()
        {
            try
            {
                // Очищаем все фильтры
                HttpContext.Session.Remove("UserFilter");
                HttpContext.Session.Remove("StartDateFilter");
                HttpContext.Session.Remove("EndDateFilter");
                HttpContext.Session.Remove("ActionFilter");

                // Очищаем все сообщения
                HttpContext.Session.Remove("UserMessage");
                HttpContext.Session.Remove("UserMessageType");
                HttpContext.Session.Remove("DateMessage");
                HttpContext.Session.Remove("DateMessageType");
                HttpContext.Session.Remove("ActionMessage");
                HttpContext.Session.Remove("ActionMessageType");

                // Очищаем стили полей
                HttpContext.Session.Remove("StrartDate");
                HttpContext.Session.Remove("EndDate");

                return RedirectToAction("Index", "LogView");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Log_Creater.Create(
                    Guid.Parse(User.Identity.Name),
                    "Filter_Clear_Error",
                    $"Error when resetting filters: {ex.Message}\n" +
                    $"Stack Trace: {ex.StackTrace}"
                );

                return RedirectToAction("Index", "LogView");
            }
        }

        public IActionResult ClearSession()
        {
            // Очищаем только служебные данные сессии, но сохраняем фильтры
            HttpContext.Session.Remove("UserMessage");
            HttpContext.Session.Remove("UserMessageType");
            HttpContext.Session.Remove("DateMessage");
            HttpContext.Session.Remove("DateMessageType");
            HttpContext.Session.Remove("ActionMessage");
            HttpContext.Session.Remove("ActionMessageType");
            HttpContext.Session.Remove("ReportMessage");
            HttpContext.Session.Remove("ReportMessageType");
            HttpContext.Session.Remove("OpenModalUser");
            HttpContext.Session.Remove("OpenModalData");
            HttpContext.Session.Remove("OpenModalAction");
            HttpContext.Session.Remove("OpenReportForm");
            HttpContext.Session.Remove("StrartDate");
            HttpContext.Session.Remove("EndDate");

            return RedirectToAction("Index", "LogView");
        }
    }
}
