using DataAccessLibrary.Models;
using Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary;
using System.Text.Json;
using ServiceLibrary;
using System.Net.Mail;
using ComputerMetricsLibrary;
using System.Text;

namespace Notification_System.Controllers
{
    public class MessengerCreateController : Controller
    {
        private readonly NotificationSystemContext _notificationSystemContext;
        private static CancellationTokenSource _cts = null;
        private int PeriodTime = 1;
        public MessengerCreateController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }

        [Authorize(Roles = "ServicesCreator,Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["ShowSideBarBlock"] = true;

                // Получаем список названий сервисов
                var service_names = await _notificationSystemContext.ServiceNames.ToListAsync();

                // Получаем полные данные сервисов с включенными зависимостями
                var service = await _notificationSystemContext.Services
                    .Include(sn => sn.ServiceName)
                    .Include(ac => ac.Account)
                        .ThenInclude(p => p.Profile)
                    .ToListAsync();

                ViewBag.ServiceName = service_names;
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Messanger_Open", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} opened the messanger tab");
                return View(service);
            }
            catch (Exception ex)
            {
                // Логирование ошибки (раскомментировать при необходимости)
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Log_Lost", ex.ToString());

                // В случае ошибки возвращаем минимально работоспособное состояние
                ViewData["ShowSideBarBlock"] = true;
                ViewBag.ServiceName = new List<ServiceName>(); // Пустой список вместо null
                return View(new List<Service>()); // Пустой список сервисов
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateService(string serviceName, string dispalyName)
        {
            HttpContext.Session.Clear();

            // Если выбран email сервис
            if (serviceName == "Email") // предполагая, что у вас есть сервис с таким именем в БД
            {
                try
                {
                    // Получаем настройки из формы (нужно добавить в форму)
                    var smtpServer = Request.Form["SmtpServer"];
                    var smtpPort = int.Parse(Request.Form["SmtpPort"]);
                    var username = Request.Form["Username"];
                    var password = Request.Form["Password"];
                    var fromEmail = Request.Form["FromEmail"];
                    var enableSsl = bool.Parse(Request.Form["EnableSsl"]);
                    var sendPeriod = int.Parse(Request.Form["SendPeriod"]);

                    // Создаем и тестируем сервис
                    var emailSettings = new EmailServiceSettings
                    {
                        SmtpServer = smtpServer,
                        SmtpPort = smtpPort,
                        Username = username,
                        Password = password,
                        FromEmail = fromEmail,
                        EnableSsl = enableSsl,
                        SendTime = sendPeriod
                    };

                    var emailService = new EmailService(emailSettings);

                    // Отправляем тестовое сообщение
                    var testMessage = new EmailMessageData
                    {
                        Destination = username,
                        Message = $"Это тестовое сообщение от сервиса {dispalyName}. Не отвечайте на него."
                    };

                    SendResult result = await emailService.SendAsync(testMessage);

                    if (!result.Success)
                    {
                        Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Email_Error", result.ErrorMessage);
                        HttpContext.Session.SetString("OpenModal", "true");

                        HttpContext.Session.SetString("Message", $"Ошибка отправки тестового сообщения: {result.ErrorMessage}");
                        HttpContext.Session.SetString("Error", "alert-error");
                        return RedirectToAction("Index", "MessengerCreate");
                    }

                    if (await AddService.AddEmail(dispalyName, Guid.Parse(User.Identity.Name),
                        smtpServer, smtpPort, username, password, fromEmail, enableSsl, sendPeriod))
                    {
                        Log_Creater.Create(Guid.Parse(User.Identity.Name), "Create_Email_Success", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} successfully added email service");
                        HttpContext.Session.SetString("OpenModal", "true");
                        HttpContext.Session.SetString("Message", "Сервис успешно добавлен и протестирован");
                        HttpContext.Session.SetString("Error", "alert-success");
                    }
                }
                catch (Exception ex)
                {
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Create_Email_Error", ex.ToString());
                    HttpContext.Session.SetString("OpenModal", "true");
                    HttpContext.Session.SetString("Message", $"Ошибка при настройке email сервиса: {ex.Message}");
                    HttpContext.Session.SetString("Error", "alert-error");
                    return RedirectToAction("Index", "MessengerCreate");
                }
            }
            
            
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail(Guid serviceID)
        {
            await TestSendAsync(serviceID);
            Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send", $"User  {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} sent a test message to the service");
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> BlockedService(Guid serviceID)
        {
            if (await AddService.Blocked(serviceID))
            {
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Blocked", $"User  {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))}  blocked the ");
                return RedirectToAction("Index", "MessengerCreate");
            }
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> UnBlockedService(Guid serviceID)
        {
            if (await AddService.UnBlocked(serviceID))
            {
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Unblocked", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} blocked the {serviceID}");
                return RedirectToAction("Index", "MessengerCreate");
            }
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> StartStopSend(Guid serviceID)
        {
            try
            {
                // Находим сервис
                var service = await _notificationSystemContext.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceID);

                if (service == null)
                    return NotFound();

                // Обработка включения/выключения сервиса
                if (service.ServiseIsDisable)
                {
                    // Включаем сервис
                    service.ServiseIsDisable = false;
                    await _notificationSystemContext.SaveChangesAsync();

                    // Запускаем фоновую задачу
                    _cts?.Dispose(); // Освобождаем предыдущий токен, если был
                    _cts = new CancellationTokenSource();
                    _ = Task.Run(() => BackgroundSendLoop(serviceID, _cts.Token));
                }
                else
                {
                    // Выключаем сервис
                    service.ServiseIsDisable = true;
                    await _notificationSystemContext.SaveChangesAsync();

                    // Останавливаем фоновую задачу
                    _cts?.Cancel();
                }

                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx)
            {
                // Логирование ошибки базы данных
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", dbEx.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при сохранении изменений в базе данных");
            }
            catch (OperationCanceledException ocEx)
            {
                // Логирование отмены задачи
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ocEx.ToString());
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логирование неожиданных ошибок
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла непредвиденная ошибка");
            }
        }
        private async Task SendEmailAsync(Guid serviceID)
        {
            NotificationSystemContext context = null;
            try
            {
                context = new NotificationSystemContext();
                var service = await context.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceID);

                if (service == null)
                {
                    // Логирование ошибки
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", $"Service not found");
                    return;
                }

                Dictionary<string, string> setting;
                try
                {
                    setting = DataHelper.DictToString.ReturnString(service.ServiceSettings);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки парсинга настроек
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                    return;
                }

                // Проверка наличия всех необходимых ключей
                var requiredKeys = new[] { "SmtpServer", "SmtpPort", "Username", "Password", "FromEmail", "EnableSsl" };
                if (requiredKeys.Any(key => !setting.ContainsKey(key)))
                {
                    // Логирование отсутствия ключей
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", $"Not all settings found");
                    return;
                }

                EmailServiceSettings emailSettings;
                try
                {
                    emailSettings = new EmailServiceSettings
                    {
                        SmtpServer = setting["SmtpServer"],
                        SmtpPort = int.Parse(setting["SmtpPort"]),
                        Username = setting["Username"],
                        Password = setting["Password"],
                        FromEmail = setting["FromEmail"],
                        EnableSsl = bool.Parse(setting["EnableSsl"]),
                        SendTime = int.Parse(setting["SendTime"])
                    };
                }
                catch (FormatException ex)
                {
                    // Логирование ошибки формата данных
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                    return;
                }
                PeriodTime = emailSettings.SendTime;
                var emailService = new EmailService(emailSettings);
                var collector = new ComputerMetricsCollector();

                Dictionary<string, string> metricsDict = collector.GetMetricsAsString();

                string metricsString = ConvertDictionaryToString(metricsDict);

                var testMessage = new EmailMessageData
                {
                    Destination = emailSettings.Username,
                    Message = metricsString
                };

                try
                {
                    SendResult result = await emailService.SendAsync(testMessage);
                    // Логирование успешной отправки (при необходимости)
                    // _logger.LogInformation($"Тестовое письмо для сервиса {serviceID} отправлено");
                }
                catch (SmtpException ex)
                {
                    // Логирование ошибки SMTP
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                }
                catch (Exception ex)
                {
                    // Логирование общей ошибки отправки
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                }
            }
            finally
            {
                context?.Dispose();
            }
        }

        private async Task TestSendAsync(Guid serviceID)
        {
            NotificationSystemContext context = null;
            try
            {
                context = new NotificationSystemContext();
                var service = await context.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceID);

                if (service == null)
                {
                    // Логирование ошибки
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", $"Service not found");
                    return;
                }

                Dictionary<string, string> setting;
                try
                {
                    setting = DataHelper.DictToString.ReturnString(service.ServiceSettings);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки парсинга настроек
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                    return;
                }

                // Проверка наличия всех необходимых ключей
                var requiredKeys = new[] { "SmtpServer", "SmtpPort", "Username", "Password", "FromEmail", "EnableSsl" };
                if (requiredKeys.Any(key => !setting.ContainsKey(key)))
                {
                    // Логирование отсутствия ключей
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", $"Not all settings found");
                    return;
                }

                EmailServiceSettings emailSettings;
                try
                {
                    emailSettings = new EmailServiceSettings
                    {
                        SmtpServer = setting["SmtpServer"],
                        SmtpPort = int.Parse(setting["SmtpPort"]),
                        Username = setting["Username"],
                        Password = setting["Password"],
                        FromEmail = setting["FromEmail"],
                        EnableSsl = bool.Parse(setting["EnableSsl"])
                    };
                }
                catch (FormatException ex)
                {
                    // Логирование ошибки формата данных
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                    return;
                }

                var emailService = new EmailService(emailSettings);

                var testMessage = new EmailMessageData
                {
                    Destination = emailSettings.Username,
                    Message = $"Это тестовое сообщение от сервиса {service.ServiceDisplayName}. Не отвечайте на него."
                };

                try
                {
                    SendResult result = await emailService.SendAsync(testMessage);
                    // Логирование успешной отправки (при необходимости)
                    // _logger.LogInformation($"Тестовое письмо для сервиса {serviceID} отправлено");
                }
                catch (SmtpException ex)
                {
                    // Логирование ошибки SMTP
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                }
                catch (Exception ex)
                {
                    // Логирование общей ошибки отправки
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                }
            }
            finally
            {
                context?.Dispose();
            }
        }

        private async Task BackgroundSendLoop(Guid serviceID, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                NotificationSystemContext context = null;
                try
                {
                    context = new NotificationSystemContext();

                    // Получаем сервис с проверкой отмены
                    var service = await context.Services
                        .AsNoTracking() // Добавляем для оптимизации
                        .FirstOrDefaultAsync(s => s.ServiceId == serviceID, token);

                    // Проверяем условия выхода
                    if (service == null || service.ServiseIsDisable)
                        break;

                    // Вызываем метод отправки письма
                    await SendEmailAsync(serviceID);

                    // Ожидаем с проверкой отмены
                    await Task.Delay(TimeSpan.FromMinutes(1*PeriodTime), token);
                }
                catch (OperationCanceledException)
                {
                    // Нормальное завершение при отмене
                    break;
                }
                catch (Exception ex)
                {
                    // Логирование ошибки (раскомментировать при необходимости)
                    // _logger.LogError(ex, $"Ошибка в фоновом процессе для сервиса {serviceID}");
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                    // Делаем паузу перед повторной попыткой
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                finally
                {
                    context?.Dispose();
                }
            }
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "MessengerCreate"); // Редирект на главную страницу сервисов
        }

        private string ConvertDictionaryToString(Dictionary<string, string> metricsDict)
        {
            var sb = new StringBuilder();

            foreach (var kvp in metricsDict)
            {
                // Пропускаем разделительные элементы ("===")
                if (kvp.Value == "===")
                {
                    sb.AppendLine(kvp.Key);
                }
                else
                {
                    sb.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
            }

            return sb.ToString();
        }
    }
}
