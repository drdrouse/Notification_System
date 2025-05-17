using DataAccessLibrary;
using DataAccessLibrary.Models;
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
    }
}
