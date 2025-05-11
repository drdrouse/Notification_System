using DataAccessLibrary.Models;
using Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> CreateService(string serviceName, string dispalyName, string link)
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

                   
                }
                catch (Exception ex)
                {
                    HttpContext.Session.SetString("OpenModal", "true");
                    HttpContext.Session.SetString("Message", $"Ошибка при настройке email сервиса: {ex.Message}");
                    HttpContext.Session.SetString("Error", "alert-error");
                    return RedirectToAction("Index", "MessengerCreate");
                }
            }

            HttpContext.Session.SetString("OpenModal", "true");
            HttpContext.Session.SetString("Message", "Сервис успешно добавлен.");
            HttpContext.Session.SetString("Error", "alert-success");
            return RedirectToAction("Index", "MessengerCreate");
        }


        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "MessengerCreate"); // Редирект на главную страницу сервисов
        }
    }
}
