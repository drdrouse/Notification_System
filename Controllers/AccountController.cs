using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notification_System.Controllers
{
   [Authorize]
    public class AccountController : Controller
    {

        private readonly NotificationSystemContext _notificationSystemContext;

        public AccountController(NotificationSystemContext notificationSystemContext)
        {
            _notificationSystemContext = notificationSystemContext;
        }
        public IActionResult Index()
        {

            ViewData["ShowSideBarBlock"] = true;
            Guid accountID = Guid.Parse(User.Identity.Name);
            var account = _notificationSystemContext.Accounts.Where(acc => acc.AccountId == accountID).FirstOrDefault();
            var profile = _notificationSystemContext.Profiles.Where(prof => prof.ProfileId == account.ProfileId).FirstOrDefault();
            return View(profile);
        }
    }
}
