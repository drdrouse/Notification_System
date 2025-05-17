using DataAccessLibrary.Models;
using Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary;
using System.Text.Json;
using ServiceLibrary;

namespace Notification_System.Controllers
{
    public class MessengerCreateController : Controller
    {
        private readonly NotificationSystemContext _notificationSystemContext;
        private static CancellationTokenSource _cts = null;

        public MessengerCreateController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }

        [Authorize(Roles = "ServicesCreator,Admin")]
        public async Task<IActionResult> Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            var service_names = await _notificationSystemContext.ServiceNames.ToListAsync();
            var service = await _notificationSystemContext.Services.Include(sn => sn.ServiceName).
                Include(ac => ac.Account).ThenInclude(p => p.Profile).ToListAsync();
            ViewBag.ServiceName = service_names;
            return View(service);
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

                    // Создаем и тестируем сервис
                    var emailSettings = new EmailServiceSettings
                    {
                        SmtpServer = smtpServer,
                        SmtpPort = smtpPort,
                        Username = username,
                        Password = password,
                        FromEmail = fromEmail,
                        EnableSsl = enableSsl
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
                        HttpContext.Session.SetString("OpenModal", "true");

                        HttpContext.Session.SetString("Message", $"Ошибка отправки тестового сообщения: {result.ErrorMessage}");
                        HttpContext.Session.SetString("Error", "alert-error");
                        return RedirectToAction("Index", "MessengerCreate");
                    }

                    if (await AddService.AddEmail(dispalyName, Guid.Parse(User.Identity.Name),
                        smtpServer, smtpPort, username, password, fromEmail, enableSsl))
                    {
                        HttpContext.Session.SetString("OpenModal", "true");
                        HttpContext.Session.SetString("Message", "Сервис успешно добавлен и протестирован");
                        HttpContext.Session.SetString("Error", "alert-success");
                    }
                }
                catch (Exception ex)
                {
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
            await SendEmailAsync(serviceID);
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> BlockedService(Guid serviceID)
        {
            if (await AddService.Blocked(serviceID)) 
                return RedirectToAction("Index", "MessengerCreate");
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> UnBlockedService(Guid serviceID)
        {
            if (await AddService.UnBlocked(serviceID))
                return RedirectToAction("Index", "MessengerCreate");
            return RedirectToAction("Index", "MessengerCreate");
        }

        [HttpPost]
        public async Task<IActionResult> StartStopSend(Guid serviceID)
        {
            var service = _notificationSystemContext.Services.FirstOrDefault(s => s.ServiceId == serviceID);
            if (service == null)
                return NotFound();

            // Если сервис отключен, запускаем процесс
            if (service.ServiseIsDisable)
            {
                service.ServiseIsDisable = false;
                await _notificationSystemContext.SaveChangesAsync();

                _cts = new CancellationTokenSource();
                _ = Task.Run(() => BackgroundSendLoop(serviceID, _cts.Token));
            }
            else // Иначе останавливаем его
            {
                service.ServiseIsDisable = true;
                await _notificationSystemContext.SaveChangesAsync();

                _cts?.Cancel();
            }

            return RedirectToAction("Index"); // или куда вы хотите
        }
        private async Task SendEmailAsync(Guid serviceID)
        {
            using (var context = new NotificationSystemContext())
            {
                var service = context.Services.Where(s => s.ServiceId == serviceID).FirstOrDefault();

                Dictionary<string, string> setting = DataHelper.DictToString.ReturnString(service.ServiceSettings);


                var emailSettings = new EmailServiceSettings
                {
                    SmtpServer = setting["SmtpServer"],
                    SmtpPort = int.Parse(setting["SmtpPort"]),
                    Username = setting["Username"],
                    Password = setting["Password"],
                    FromEmail = setting["FromEmail"],
                    EnableSsl = bool.Parse(setting["EnableSsl"])
                };

                var emailService = new EmailService(emailSettings);

                // Отправляем тестовое сообщение
                var testMessage = new EmailMessageData
                {
                    Destination = emailSettings.Username,
                    Message = $"Это тестовое сообщение от сервиса {service.ServiceDisplayName}. Не отвечайте на него."
                };

                SendResult result = await emailService.SendAsync(testMessage);
            }
        }

        private async Task BackgroundSendLoop(Guid serviceID, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using (var context = new NotificationSystemContext())
                {
                    var service = context.Services.FirstOrDefault(s => s.ServiceId == serviceID);

                    if (service == null || service.ServiseIsDisable)
                        break;

                    // Вызов метода отправки письма
                    await SendEmailAsync(serviceID);

                    await Task.Delay(TimeSpan.FromMinutes(1), token);
                }
            }

        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "MessengerCreate"); // Редирект на главную страницу сервисов
        }
    }
}
