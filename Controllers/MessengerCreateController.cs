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
    }
}
