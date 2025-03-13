using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    public class MessengerCreateController : Controller
    {
        [Authorize(Roles = "ServicesCreator,Admin")]
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateService(string serviceName, string dispalyName, string link)
        {
            HttpContext.Session.Clear();
            HttpContext.Session.SetString("OpenModal", "true");
            return RedirectToAction("Index", "Setting");
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "MessengerCreate"); // Редирект на главную страницу сервисов
        }
    }
}
