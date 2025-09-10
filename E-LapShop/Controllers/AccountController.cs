
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "تم إنشاء الحساب بنجاح! يرجى تسجيل الدخول الآن";
                    return RedirectToAction("Auth", "Account");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View("Auth");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Auth()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    
                    // Check if user is admin
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        TempData["AdminWelcomeMessage"] = $"مرحباً بك في لوحة التحكم {user?.FullName}! تم تسجيل الدخول بنجاح";
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        TempData["WelcomeMessage"] = $"مرحباً بك {user?.FullName}! تم تسجيل الدخول بنجاح";
                        return RedirectToAction("Index", "Furni");
                    }
                }

                ModelState.AddModelError("", "Invalid login attempt");
            }

            return View("Auth");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Furni");
        }
    }
}
