using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    public class AuthorisationController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = false; 
            return View();
        }
    }
}
