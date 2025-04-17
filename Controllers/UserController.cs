using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DataAccessLibrary;

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

        public IActionResult Index(int Tabnum)
        {
            ViewData["ShowSideBarBlock"] = true;
            var statuses = _notificationSystemContext.AccountStatuses.ToList();
            var roles = _notificationSystemContext.Roles.ToList();
            var profile = _notificationSystemContext.Profiles.Where(p => p.ProfileTabNum == Tabnum).FirstOrDefault();
            var account = _notificationSystemContext.Accounts.Where(a => a.ProfileId == profile.ProfileId).
                Include(a => a.Profile).ThenInclude(p => p.Mail).ThenInclude(m => m.TypeMail).
                Include(a => a.Profile).ThenInclude(p => p.Phones).ThenInclude(ph => ph.TypePhone).
                Include(a => a.AccountStatus).
                Include(a => a.Profile).ThenInclude(p => p.Logs).ThenInclude(e => e.EventCode).
                Include(a => a.RoleAssignments).ThenInclude(ra => ra.Role).ToList();
            

            ViewBag.Statuses = statuses;
            ViewBag.Roles = roles;
            return View(account);
        }


        public async Task<IActionResult> RemoveRole(string name, int tabnum)
        {
            if (Role_Change.Remove_Role(name))
            {
                var users = User as ClaimsPrincipal;
                var identity = User.Identity as ClaimsIdentity;

                var claimToRemove = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == name);
                
                if (claimToRemove != null)
                {
                    identity.RemoveClaim(claimToRemove);
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddHours(1)
                        }
                    );
                }

                return RedirectToAction("Index", "User", new { Tabnum = tabnum });
            }
            return RedirectToAction("Index", "User",  new { Tabnum = tabnum }); 
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string status, int tabnummner)
        {
            if(Status_Change.NewStatus(status, tabnummner)) 
                return RedirectToAction("Index", "UserCreate");
            return RedirectToAction("Index", "UserCreate");
        }


        [HttpPost]
        public async Task<IActionResult> AddNewRole(int tabNum, bool LogViewer,
            bool ServicesCreator, bool UserCreator, bool Admin)
        {
            List<string> roles = new List<string>();
            if (LogViewer)
                roles.Add("LogViewer");
            if (ServicesCreator)
                roles.Add("ServicesCreator");
            if (UserCreator)
                roles.Add("ServicesCreator");
            if (Admin)
                roles.Add("Admin");
            if(Role_Change.Add_Role(roles, tabNum)) 
                return RedirectToAction("Index", "User", new { Tabnum = tabNum });
            return RedirectToAction("Index", "User", new { Tabnum = tabNum });
        }
        public async Task<IActionResult> ClearSession(int tabnum)
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "User", new { Tabnum = tabnum }); // Редирект на главную страницу сервисов
        }
    }
}
