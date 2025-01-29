using Authentication___Authorization.Data;
using Authentication___Authorization.Models;
using Authentication___Authorization.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication___Authorization.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Register()
        {


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
               return View(viewModel);
            }
            var user = new ApplicationUser()
            {
                UserName= viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                BirthDate = viewModel.BirthDate,
            };
            var result = await _userManager.CreateAsync(user,viewModel.Password);
            if (!result.Succeeded)
            {
                return View(viewModel);
            }
                return RedirectToAction(nameof(Index));
        }
          
           
        
    }
}
