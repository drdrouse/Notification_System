using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                ViewData["ShowSideBarBlock"] = true;

                if (!User.Identity.IsAuthenticated)
                {
                    // Возвращаем представление без данных, если пользователь не аутентифицирован
                    return View();
                }

                Guid accountID = Guid.Parse(User.Identity.Name);
                var account = _notificationSystemContext.Accounts
                    .FirstOrDefault(acc => acc.AccountId == accountID);

                if (account == null)
                {
                    // Возвращаем представление без данных, если аккаунт не найден
                    return View();
                }

                var profile = _notificationSystemContext.Profiles
                    .Include(mail => mail.Mail)
                        .ThenInclude(tmail => tmail.TypeMail)
                    .Include(phone => phone.Phones)
                        .ThenInclude(tphone => tphone.TypePhone)
                    .FirstOrDefault(prof => prof.ProfileId == account.ProfileId);

                return View(profile);
            }
            catch (Exception ex)
            {
                
                Log_Creater.Create(Guid.Parse(User.Identity.Name), "Profile_Lost", ex.ToString());

                // Возвращаем представление без данных в случае ошибки
                return View();
            }
        }
    }
}
