using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
    public class UserController : Controller
    {

        private readonly NotificationSystemContext _notificationSystemContext;

        public UserController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }

        [Authorize(Roles = "UserCreator,Admin")]

        public IActionResult Index(int tabnum)
        {
            ViewData["ShowSideBarBlock"] = true;
            //var profile = _notificationSystemContext.Profiles.Where(p => p.ProfileTabNum == tabnum).FirstOrDefault();
            //var account = _notificationSystemContext.Accounts.Where(a => a.ProfileId == profile.ProfileId).FirstOrDefault();
            return View();
        }
    }
}
