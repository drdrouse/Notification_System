using CsvHelper;
using DataAccessLibrary;
using DataAccessLibrary.Models;
using DataHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;
using Parquet;
using Parquet.Schema;
using Parquet.Data;

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
            try
            {
                // Очищаем только связанные с этим фильтром сессии
                HttpContext.Session.Remove("ActionMessageType");
                HttpContext.Session.Remove("ActionMessage");
                HttpContext.Session.Remove("ActionFilter");

                if (string.IsNullOrEmpty(selectedAction) || selectedAction == "-- Все события --")
                {
                    HttpContext.Session.SetString("ActionMessageType", "alert-info"); // Изменено на info для единообразия
                    HttpContext.Session.SetString("ActionMessage", "Фильтр по событиям сброшен");
                    HttpContext.Session.Remove("ActionFilter"); // Явно удаляем фильтр
                }
                else
                {
                    // Дополнительная проверка существования действия
                    if (!_notificationSystemContext.EventCodes.Any(e => e.EventCodeName == selectedAction))
                    {
                        HttpContext.Session.SetString("ActionMessageType", "alert-warning");
                        HttpContext.Session.SetString("ActionMessage", "Выбранное событие не найдено");
                    }
                    else
                    {
                        HttpContext.Session.SetString("ActionMessageType", "alert-success");
                        HttpContext.Session.SetString("ActionMessage", $"Фильтр по событию '{selectedAction}' успешно применён");
                        HttpContext.Session.SetString("ActionFilter", selectedAction);
                    }
                }

                HttpContext.Session.SetString("OpenModalAction", "true");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Log_Creater.Create(
                    Guid.Parse(User.Identity.Name),
                    "Filter_Error",
                    $"Error ActionFilter: {ex.Message}\n" +
                    $"Selected action: {selectedAction}\n" +
                    $"Stack Trace: {ex.StackTrace}"
                );

                // Устанавливаем сообщение об ошибке для пользователя
                HttpContext.Session.SetString("ActionMessageType", "alert-danger");
                HttpContext.Session.SetString("ActionMessage", "Произошла ошибка при обработке фильтра событий");
                HttpContext.Session.SetString("OpenModalAction", "true");

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult CreateReport(string reportFormat)
        {
            try
            {
                if (string.IsNullOrEmpty(reportFormat) || reportFormat == "-- Выберите формат --")
                {
                    HttpContext.Session.SetString("ReportMessageType", "alert-error");
                    HttpContext.Session.SetString("ReportMessage", "Необходимо выбрать формат отчёта");
                    HttpContext.Session.SetString("OpenReportForm", "true");
                    return RedirectToAction("Index");
                }

                // Получаем отфильтрованные данные
                IQueryable<Log> query = ApplyFilters(_notificationSystemContext.Logs); // Вынесено в метод

                var filteredData = query.ToList();

                // Генерация отчёта в выбранном формате
                byte[] reportBytes;
                string contentType;
                string fileExtension;
                string downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"
                );

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                switch (reportFormat.ToUpper())
                {
                    case "XML":
                        (reportBytes, contentType, fileExtension) = GenerateXmlReport(filteredData);
                        // Сохраняем файл на сервере
                        
                        break;
                    case "JSON":
                        (reportBytes, contentType, fileExtension) = GenerateJsonReport(filteredData);
                        break;
                    case "CSV":
                        (reportBytes, contentType, fileExtension) = GenerateCsvReport(filteredData);
                        break;
                    case "PARQUET":
                        (reportBytes, contentType, fileExtension) = GenerateParquetReport(filteredData);
                        break;
                    default:
                        throw new ArgumentException("Неподдерживаемый формат отчёта");
                }

                string serverFileName = Path.Combine(downloadsPath, $"Отчет_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}");
                System.IO.File.WriteAllBytesAsync(serverFileName, reportBytes);

                HttpContext.Session.SetString("ReportMessageType", "alert-success");
                HttpContext.Session.SetString("ReportMessage", "Отчет успешно создан");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log_Creater.Create(
                    Guid.Parse(User.Identity.Name),
                    "Error_Report",
                    $"Error: {ex}.\nWhen creating a report in format: {reportFormat}"
                );
                HttpContext.Session.SetString("ReportMessageType", "alert-error");
                HttpContext.Session.SetString("ReportMessage", $"Error during report generation: {ex.Message}");
                HttpContext.Session.SetString("OpenReportForm", "true");
                return RedirectToAction("Index");
            }
        }

        private IQueryable<Log> ApplyFilters(IQueryable<Log> query)
        {
            query = query
                .Include(p => p.Profile)
                .Include(e => e.EventCode)
                .OrderByDescending(l => l.LogDateTime);

            // Фильтр по пользователю
            string userFilter = HttpContext.Session.GetString("UserFilter");
            if (!string.IsNullOrEmpty(userFilter) && int.TryParse(userFilter, out int userId))
            {
                query = query.Where(l => l.Profile.ProfileTabNum == userId);
            }

            // Фильтр по дате
            string startDateStr = HttpContext.Session.GetString("StartDateFilter");
            string endDateStr = HttpContext.Session.GetString("EndDateFilter");

            if (DateTime.TryParseExact(startDateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                query = query.Where(l => l.LogDateTime >= startDate);
            }

            if (DateTime.TryParseExact(endDateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                query = query.Where(l => l.LogDateTime <= endDate.AddDays(1));
            }

            // Фильтр по событию
            string actionFilter = HttpContext.Session.GetString("ActionFilter");
            if (!string.IsNullOrEmpty(actionFilter))
            {
                query = query.Where(l => l.EventCode.EventCodeName == actionFilter);
            }

            return query;
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

        private (byte[], string, string) GenerateXmlReport(List<Log> data)
        {
            // Создаем XML документ
            XDocument xmlDocument = new XDocument(
                new XElement("Logs",
                    from log in data
                    select new XElement("Log",
                        new XElement("Id", log.LogId),
                        new XElement("DateTime", log.LogDateTime),
                        new XElement("User",
                            new XElement("TabNum", log.Profile?.ProfileTabNum),
                            new XElement("Name", $"{log.Profile?.ProfileSurname} {log.Profile?.ProfileName}")
                        ),
                        new XElement("Event", log.EventCode?.EventCodeName),
                        new XElement("Description", log.EventCode?.EventCodeDescription)
                    )
                )
            );

            // Конвертируем в массив байтов
            using (var memoryStream = new MemoryStream())
            {
                xmlDocument.Save(memoryStream);
                return (memoryStream.ToArray(), "application/xml", ".xml");
            }
        }

        private (byte[], string, string) GenerateJsonReport(List<Log> data)
        {
            var jsonData = data.Select(log => new
            {
                Id = log.LogId,
                DateTime = log.LogDateTime,
                User = new
                {
                    TabNum = log.Profile?.ProfileTabNum,
                    Name = $"{log.Profile?.ProfileSurname} {log.Profile?.ProfileName}"
                },
                Event = log.EventCode?.EventCodeName,
                Description = log.EventCode?.EventCodeDescription
            });

            string json = JsonSerializer.Serialize(new { Logs = jsonData }, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            return (Encoding.UTF8.GetBytes(json), "application/json", ".json");
        }

        private (byte[], string, string) GenerateCsvReport(List<Log> data)
        {
            var sb = new StringBuilder();

            // Заголовки
            sb.AppendLine("Id,DateTime,TabNum,UserName,Event,Description");

            // Данные
            foreach (var log in data)
            {
                var tabNum = log.Profile?.ProfileTabNum.ToString() ?? "";
                var userName = $"{log.Profile?.ProfileSurname} {log.Profile?.ProfileName}";
                var eventName = log.EventCode?.EventCodeName ?? "";
                var description = log.EventCode?.EventCodeDescription ?? "";

                sb.AppendLine($"\"{log.LogId}\",\"{log.LogDateTime}\",\"{tabNum}\",\"{userName}\",\"{eventName}\",\"{description}\"");
            }

            return (Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", ".csv");
        }

        private (byte[], string, string) GenerateParquetReport(List<Log> data)
        {

            // Реализация для Parquet будет сложнее, может потребоваться дополнительная библиотека
            // Например, используя Parquet.Net
            throw new NotImplementedException("Parquet generation not implemented yet");
        }
    }
}
