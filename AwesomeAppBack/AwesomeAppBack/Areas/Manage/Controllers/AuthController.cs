using AwesomeAppBack.Areas.Manage.ViewModels;
using AwesomeAppBack.DAL;
using AwesomeAppBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AwesomeAppBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AuthController : Controller
    {
        private Context Context { get; }
        private UserManager<AppUser> UserManager { get; }
        private SignInManager<AppUser> SignInManager { get; }
        public AuthController(Context context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            Context = context;
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInVm signInVm)
        {
            AppUser user = await UserManager.FindByEmailAsync(signInVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(signInVm);
            }
            var result = await SignInManager.PasswordSignInAsync(user, signInVm.Password, true, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "You have exceeded the password entry limit");
                return View(signInVm);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password Incorrect!");
                return View(signInVm);
            }
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

   

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm registerVm)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = new AppUser
            {
                UserName = registerVm.UserName,
                Email = registerVm.Email,
            };
            IdentityResult result = await UserManager.CreateAsync(user, registerVm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    return View();
                }
            }
            await SignInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SignOut()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }
    }
}
