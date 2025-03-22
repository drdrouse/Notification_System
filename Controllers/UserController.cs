using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var statuses = _notificationSystemContext.AccountStatuses.ToList();
            var profile = _notificationSystemContext.Profiles.Where(p => p.ProfileTabNum == tabnum).FirstOrDefault();
            var account = _notificationSystemContext.Accounts.Where(a => a.ProfileId == profile.ProfileId).
                Include(a => a.Profile).ThenInclude(p => p.Mail).ThenInclude(m => m.TypeMail).
                Include(a => a.Profile).ThenInclude(p => p.Phones).ThenInclude(ph => ph.TypePhone).
                Include(a => a.AccountStatus).
                Include(a => a.Profile).ThenInclude(p => p.Logs).ThenInclude(e => e.EventCode).ToList();

            ViewBag.Statuses = statuses;
            return View(account);
        }

        //[HttpPost]
        //public async Task<IActionResult> ChangeStatus(string status)
        //{

        //}
    }
}
