
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        TempData["AdminWelcomeMessage"] = $"مرحباً بك في لوحة التحكم {user?.FullName}! تم تسجيل الدخول بنجاح";
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (user != null)
                    {
                        // Ensure FullName claim exists/updated for UI display
                        var claims = await _userManager.GetClaimsAsync(user);
                        var fullNameClaim = claims.FirstOrDefault(c => c.Type == "FullName");
                        if (fullNameClaim != null)
                        {
                            if (fullNameClaim.Value != (user?.FullName ?? string.Empty))
                                await _userManager.ReplaceClaimAsync(user, fullNameClaim, new Claim("FullName", user?.FullName ?? string.Empty));
                        }
                        else
                        {
                            await _userManager.AddClaimAsync(user, new Claim("FullName", user?.FullName ?? string.Empty));
                        }
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Auth");

            var model = new E_LapShop.Models.ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(E_LapShop.Models.ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Auth");

            user.FullName = model.FullName ?? user.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Update or add FullName claim
                var claims = await _userManager.GetClaimsAsync(user);
                var fullNameClaim = claims.FirstOrDefault(c => c.Type == "FullName");
                if (fullNameClaim != null)
                {
                    if (fullNameClaim.Value != (user.FullName ?? string.Empty))
                        await _userManager.ReplaceClaimAsync(user, fullNameClaim, new Claim("FullName", user.FullName ?? string.Empty));
                }
                else
                {
                    await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName ?? string.Empty));
                }
                // Refresh sign-in so updated claims/values reflect immediately in UI
                await _signInManager.RefreshSignInAsync(user);
                
                TempData["Success"] = "تم تحديث بيانات الحساب بنجاح! ✅";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                TempData["Error"] = "حدث خطأ أثناء تحديث البيانات: " + error.Description;
            }

            return View(model);
        }
    }
}
