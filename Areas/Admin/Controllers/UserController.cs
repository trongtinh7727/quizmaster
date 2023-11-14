using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.Models;

namespace QuizMaster.Areas.Admin.Controllers
{
    [Area("Admin")]
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

    }
}
