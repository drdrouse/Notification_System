using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Notification_System.Controllers
{


    public class UserCreateController : Controller
    {

        private readonly NotificationSystemContext _notificationSystemContext;

        public UserCreateController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }


        [Authorize(Roles = "UserCreator,Admin")]
        public IActionResult Index()
        {
            ViewData["ShowSideBarBlock"] = true;
            var profiles = _notificationSystemContext.Accounts.
                Include(ac => ac.Profile).ThenInclude(p => p.Phones).
                Include(ac => ac.Profile).ThenInclude(p => p.Mail).ThenInclude(m => m.TypeMail).
                Include(ac => ac.RoleAssignments).ThenInclude(r => r.Role).
                Include(ac => ac.AccountStatus);
            return View(profiles);
        }
    }
}
