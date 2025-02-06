using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    public class UserCreateController : Controller
    {
        [Authorize(Roles = "UserCreator")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
