using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RabbitMq.ExcelCreate.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string mail,string password)
        {
            var user = await _userManager.FindByEmailAsync(mail);

            if (user == null)
            {
                return View();
            }
            else
            {
                var giris = await _signInManager.PasswordSignInAsync(user, password, true, false);
                if(giris.Succeeded)
                {
                    return RedirectToAction("Index","Product");
                }
            }
                
                
                
            return View();
        }
    }
}
