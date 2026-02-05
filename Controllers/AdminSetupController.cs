using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace LibraryMS.Controllers
{
    public class AdminSetupController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminSetupController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("Member"))
                await _roleManager.CreateAsync(new IdentityRole("Member"));

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Content($"Tebrikler {user.UserName}! Artık 'Admin' yetkisine sahipsin. Çıkış yapıp tekrar girmen gerekebilir.");
            }

            return Content("Lütfen önce sisteme giriş yapın, sonra bu sayfayı çalıştırın!");
        }
    }
}