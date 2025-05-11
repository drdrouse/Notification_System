using DataAccessLibrary.Models;
using Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary;

namespace Notification_System.Controllers
{
    public class MessengerCreateController : Controller
    {
        private readonly NotificationSystemContext _notificationSystemContext;

        public MessengerCreateController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }

        [Authorize(Roles = "ServicesCreator,Admin")]
        public async Task<IActionResult> Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            var service_names = await _notificationSystemContext.ServiceNames.ToListAsync();
            return View(service_names);
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

                    var result = emailService.Send(testMessage);

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


        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "MessengerCreate"); // Редирект на главную страницу сервисов
        }
    }
}
