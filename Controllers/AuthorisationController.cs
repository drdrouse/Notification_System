using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    [AllowAnonymous]
    public class AuthorisationController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = false; 
            return View();
        }
    }
}
