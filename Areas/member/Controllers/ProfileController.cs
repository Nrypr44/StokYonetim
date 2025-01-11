using EntityLayer.Concrete;
using InventoryManagement.Areas.member.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Areas.member.Controllers
{
    [Area("member")]
    [Authorize]  // Bu kontrolördeki tüm işlemler sadece giriş yapmış kullanıcılar tarafından yapılabilir.
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Profil görüntüleme ve düzenleme sayfası
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new EditProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcı adı ve e-posta güncellemesi
            user.UserName = model.UserName;
            user.Email = model.Email;

            // Eğer kullanıcı şifreyi değiştirmek istiyorsa
            if (!string.IsNullOrEmpty(model.Password))
            {
                // Şifreler uyuşuyorsa, yeni şifreyi değiştireceğiz
                if (model.Password == model.ConfirmPassword)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Şifre güncellenirken bir hata oluştu.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Yeni şifre ve şifre tekrar uyuşmuyor.");
                    return View(model);
                }
            }
            else
            {
                // Şifre değiştirilmek istenmiyorsa, eski şifreyi kullanarak işlem yapıyoruz
                var oldPasswordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!oldPasswordCheck)
                {
                    ModelState.AddModelError("", "Eski şifreniz yanlış.");
                    return View(model);
                }

                // Eski şifreyi kullanmaya devam et
                // Eğer şifre değiştirilmiyorsa, mevcut şifreyi koruyacağız
                user.PasswordHash = user.PasswordHash;
            }

            // Güncelleme işlemi
            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                TempData["Success"] = "Profil başarıyla güncellendi!";
                return RedirectToAction("EditProfile");
            }
            else
            {
                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu.");
                return View(model);
            }
        }

    }
}

    