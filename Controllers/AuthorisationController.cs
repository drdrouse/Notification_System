using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    [AllowAnonymous]
    public class AuthorisationController : Controller
    {
        [HttpPost]
        public IActionResult Index(string login, string password)
        {
            if (login == "check" && password == "123456")
            {
                return RedirectToAction("Index", "Account");
            }

            ViewBag.Error = "Неверно введены данные";
            return View();

        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = false; 
            return View();
        }
    }
}
