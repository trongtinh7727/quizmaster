using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.Areas.Admin.Viewmodel;
using QuizMaster.Models;

namespace QuizMaster.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<QuizMasterUser> _userManager;

        public UserController(UserManager<QuizMasterUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();


            return View(users);
        }

        public async Task<IActionResult> Statistics()
        {

            //tong so tk tao 7 ngay gan nhat 
            var currentDate = DateTime.Now;
            var sevenDaysAgo = currentDate.AddDays(-7);
            var newAccountsIn7Days = _userManager.Users
                .Where(u => u.CreatedAt >= sevenDaysAgo && u.CreatedAt <= currentDate)
                .Count();
            // so tai khoan hoat dong
            var activeAccounts = _userManager.Users
            .Where(u => u.LockoutEnd == null || u.LockoutEnd <= currentDate)
            .Count();
            //so tai khoan bi lock
            var lockedAccounts = _userManager.Users
                .Where(u => u.LockoutEnd != null && u.LockoutEnd > currentDate)
                .Count();
            //
            var currentYear = currentDate.Year;
            var currentMonth = currentDate.Month;
            var accountCountsByMonth = new List<int>();
            var total = _userManager.Users
                    .Where(u => u.CreatedAt.Year < currentYear)
                    .Count();
            for (int month = 1; month <= currentMonth; month++)
            {
                var accountsInMonth = total + _userManager.Users
                    .Where(u => u.CreatedAt.Year == currentYear && u.CreatedAt.Month == month)
                    .Count();
                total = accountsInMonth;

                accountCountsByMonth.Add(accountsInMonth);
            }
            var a = accountCountsByMonth;
            var data = new UserStaticsViewModel
            {
                NewAccountsIn7Days = newAccountsIn7Days,
                ActiveAccounts = activeAccounts,
                LockedAccounts = lockedAccounts,
                AccountCountsByMonth = accountCountsByMonth
            };
            return View(data);
        }

        // POST: Admin/User/Lock
        [HttpPost]
        public async Task<IActionResult> Lock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Lock the user for a specified period or indefinitely
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(10));
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/User/Unlock
        [HttpPost]
        public async Task<IActionResult> Unlock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
