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
            try
            {
                ViewData["ShowSideBarBlock"] = true;

                var statuses = _notificationSystemContext.AccountStatuses.ToList();
                var roles = _notificationSystemContext.Roles.ToList();

                var profile = _notificationSystemContext.Profiles
                    .FirstOrDefault(p => p.ProfileTabNum == Tabnum);

                if (profile == null)
                {
                    ViewBag.Statuses = statuses;
                    ViewBag.Roles = roles;
                    return View(new List<Account>());
                }

                var account = _notificationSystemContext.Accounts
                    .Where(a => a.ProfileId == profile.ProfileId)
                    .Include(a => a.Profile).ThenInclude(p => p.Mail).ThenInclude(m => m.TypeMail)
                    .Include(a => a.Profile).ThenInclude(p => p.Phones).ThenInclude(ph => ph.TypePhone)
                    .Include(a => a.AccountStatus)
                    .Include(a => a.Profile).ThenInclude(p => p.Logs).ThenInclude(e => e.EventCode)
                    .Include(a => a.RoleAssignments).ThenInclude(ra => ra.Role)
                    .ToList();

                ViewBag.Statuses = statuses;
                ViewBag.Roles = roles;
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "User_Open", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} opened user view");
                return View(account);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем минимально работоспособное состояние
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "User_Lost", ex.ToString());
                ViewData["ShowSideBarBlock"] = true;
                ViewBag.Statuses = new List<AccountStatus>();
                ViewBag.Roles = new List<Role>();
                return View(new List<Account>());
            }
        }


        public async Task<IActionResult> RemoveRole(string name, int tabnum)
        {
            try
            {
                if (Role_Change.Remove_Role(name))
                {
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Remove_Role_Success", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} successfully deleted the role from the user {tabnum}");
                    return RedirectToAction("Index", "User", new { Tabnum = tabnum });
                }

                return RedirectToAction("Index", "User", new { Tabnum = tabnum });
            }
            catch (Exception ex)
            {
                // В случае ошибки все равно перенаправляем на ту же страницу
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Romove_Role_Error", ex.ToString());
                return RedirectToAction("Index", "User", new { Tabnum = tabnum });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string status, int tabnummner)
        {
            try
            {
                if (Status_Change.NewStatus(status, tabnummner))
                {
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Change_Status_Success", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} has successfully changed the role of the user {tabnummner}");
                    return RedirectToAction("Index", "UserCreate");
                }
                return RedirectToAction("Index", "UserCreate");
            }
            catch (Exception ex)
            {
                // Логирование ошибки (раскомментировать при необходимости)
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Change_Status_Error", ex.ToString());

                // В случае ошибки все равно перенаправляем на ту же страницу
                return RedirectToAction("Index", "UserCreate");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddNewRole(int tabNum, bool LogViewer,
            bool ServicesCreator, bool UserCreator, bool Admin)
        {
            try
            {
                List<string> roles = new List<string>();
                if (LogViewer)
                    roles.Add("LogViewer");
                if (ServicesCreator)
                    roles.Add("ServicesCreator");
                if (UserCreator)
                    roles.Add("UserCreator");
                if (Admin)
                    roles.Add("Admin");

                if (Role_Change.Add_Role(roles, tabNum))
                {
                    Log_Creater.Create(Guid.Parse(User.Identity.Name), "Add_Role_Success", $"User {Log_Creater.TabNum(Guid.Parse(User.Identity.Name))} successfully added the role to the user {tabNum}");
                    return RedirectToAction("Index", "User", new { Tabnum = tabNum });
                }
                return RedirectToAction("Index", "User", new { Tabnum = tabNum });
            }
            catch (Exception ex)
            {
                // Логирование ошибки (раскомментировать при необходимости)
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Error adding a role", ex.ToString());

                // В случае ошибки все равно перенаправляем на ту же страницу
                return RedirectToAction("Index", "User", new { Tabnum = tabNum });
            }
        }
        public async Task<IActionResult> ClearSession(int tabnum)
        {
            HttpContext.Session.Clear(); // Очищаем всю сессию
            return RedirectToAction("Index", "User", new { Tabnum = tabnum }); // Редирект на главную страницу сервисов
        }
    }
}
