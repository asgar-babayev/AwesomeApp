using AwesomeAppBack.Areas.Manage.ViewModels;
using AwesomeAppBack.DAL;
using AwesomeAppBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
            }
            var result = await SignInManager.PasswordSignInAsync(user, signInVm.Password, false,false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password Incorrect!");
            }
            return RedirectToAction("Index");
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

        private string GenerateJSONWebToken(string username)
        {
            var claims = new[] {
                new Claim("Username", username)
            };
            var key = "HelloAwesomeAppIAmDeveloper@private-6-6-6";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: "https://localhost",
                audience: "https://localhost",
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials,
                claims: claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
