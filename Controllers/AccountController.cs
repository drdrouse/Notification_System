using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            return View();
        }
    }
}
