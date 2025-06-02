using ComputerMetricsLibrary;
using DataAccessLibrary;
using DataAccessLibrary.Models;
using Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLibrary;
using System.Collections.Concurrent;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Notification_System.Controllers
{
    public class MessengerCreateController : Controller
    {
        private readonly NotificationSystemContext _notificationSystemContext;
        private static readonly ConcurrentDictionary<Guid, CancellationTokenSource> _serviceCancellationTokens =
            new ConcurrentDictionary<Guid, CancellationTokenSource>();
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
        public async Task<IActionResult> DeleteService(Guid serviceID)
        {
            using (var context = new NotificationSystemContext())
            {
                var service = context.Services.Where(s => s.ServiceId == serviceID).FirstOrDefault();

                string serviceName = service.ServiceDisplayName;

                if (await AddService.Delete(serviceID))
                {
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Delete_Service", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} delete service {serviceName}");
                    return RedirectToAction("Index", "MessengerCreate");
                }
            }
                
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> StartStopSend(Guid serviceID)
        {
            try
            {
                var service = await _notificationSystemContext.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceID);

                if (service == null)
                    return NotFound();

                if (service.ServiseIsDisable)
                {
                    // Включаем сервис
                    service.ServiseIsDisable = false;
                    await _notificationSystemContext.SaveChangesAsync();

                    // Останавливаем предыдущую задачу, если она была
                    if (_serviceCancellationTokens.TryRemove(serviceID, out var existingCts))
                    {
                        existingCts.Cancel();
                        existingCts.Dispose();
                    }

                    // Создаем новый CTS для этого сервиса
                    var newCts = new CancellationTokenSource();
                    _serviceCancellationTokens.TryAdd(serviceID, newCts);

                    // Запускаем фоновую задачу
                    _ = Task.Run(() => BackgroundSendLoop(serviceID, newCts.Token));
                }
                else
                {
                    // Выключаем сервис
                    service.ServiseIsDisable = true;
                    await _notificationSystemContext.SaveChangesAsync();

                    // Останавливаем фоновую задачу
                    if (_serviceCancellationTokens.TryRemove(serviceID, out var cts))
                    {
                        cts.Cancel();
                        cts.Dispose();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла ошибка");
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
                var requiredKeys = new[] { "SmtpServer", "SmtpPort", "Username", "Password", "FromEmail", "EnableSsl", "SendTime" };
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
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using (var context = new NotificationSystemContext())
                    {
                        var service = await context.Services
                            .AsNoTracking()
                            .FirstOrDefaultAsync(s => s.ServiceId == serviceID, token);

                        if (service == null || service.ServiseIsDisable)
                            break;

                        await SendEmailAsync(serviceID);

                        var delayTime = TimeSpan.FromMinutes(1 * await GetTime(serviceID));
                        await Task.Delay(delayTime, token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальное завершение
            }
            catch (Exception ex)
            {
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Send_Error", ex.ToString());
            }
            finally
            {
                _serviceCancellationTokens.TryRemove(serviceID, out _);
            }
        }


        private async Task<int> GetTime(Guid serviceID)
        {
            int SendPeriod = 1;

            NotificationSystemContext context = new NotificationSystemContext();
            var service = await context.Services
                .FirstOrDefaultAsync(s => s.ServiceId == serviceID);

            Dictionary<string, string> setting;
            setting = DataHelper.DictToString.ReturnString(service.ServiceSettings);       

            SendPeriod = int.Parse(setting["SendTime"]);
            return SendPeriod;
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
