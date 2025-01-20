using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            return View();
        }
    }
}
