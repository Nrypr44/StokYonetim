using EntityLayer.Concrete;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // Register method here...
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email // E-posta adresi ile username aynıysa bu şekilde tutabilirsiniz
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Varsayılan olarak 'User' rolünü kullanıcıya atayalım
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (roleResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // Login method
        [HttpGet]
        public IActionResult Login()
        {
            // Boş bir model gönderiliyor
            return View(new LoginViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // ModelState geçerli değilse, hataları view'a gönderiyoruz
            if (!ModelState.IsValid)
            {
                return View(model);   // Hataları view'a geri gönderiyoruz.
            }

            // Eğer model null ya da Username boşsa hata mesajı göster
            if (string.IsNullOrEmpty(model.Username))
            {
                ModelState.AddModelError("", "Username cannot be empty.");
                return View(model);
            }

            // Kullanıcı adı ile kullanıcıyı bulma işlemi
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Eğer kullanıcı bulunamadıysa hata mesajı ekle
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
                return View(model);
            }

            // Şifreyi doğrulama işlemi
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {

                if (User.IsInRole("User"))
                {
                    return RedirectToAction("AddSale", "UserSale", new { area = "member" });
                }
                else if(User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Product");

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }

            }

            // Eğer giriş başarısızsa, hata mesajını ekleyin
            ModelState.AddModelError("", "Geçersiz giriş.");
            return View(model);  // Hatalarla birlikte view'a geri dönüyoruz.
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UpdateProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.UserName = model.Username;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Profil başarıyla güncellendi.";
                return View(model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // Logout method
        public async Task<IActionResult> Logout()
        {
            // Kullanıcıyı çıkış yaptırır
            await HttpContext.SignOutAsync();

            // Giriş sayfasına veya başka bir sayfaya yönlendir
            return RedirectToAction("Login", "Account");
        }
    }
}
