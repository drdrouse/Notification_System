using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var log = _notificationSystemContext.Logs.Include(p => p.Profile).
                Include(e => e.EventCode).ToList();
            ViewData["ShowSideBarBlock"] = true;
            return View(log);
        }
    }
}
