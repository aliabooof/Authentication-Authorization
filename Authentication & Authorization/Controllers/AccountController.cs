using Authentication___Authorization.Data;
using Authentication___Authorization.Interfaces;
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
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
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
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account",
                    new { userId = user.Email, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by clicking <a href='{callbackUrl}'>here</a>.");

       

                return View("RegistrationSuccessful"); // Prompt user to check their email

                //return RedirectToAction(nameof(Login));
            }
                return View(viewModel);
        }
        

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            
            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(viewModel);
            }

            
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "You need to confirm your email to log in.");
                return View(viewModel);
            }

           
                var flag = await _signInManager.CheckPasswordSignInAsync(user, viewModel.Password,false);

            if (flag.Succeeded)
            {
                var result = await _signInManager.PasswordSignInAsync(user,viewModel.Password,viewModel.RememberMe,false);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index),"Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(viewModel);

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
   

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var user = await _userManager.FindByEmailAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            return NotFound($"Unable to load user with email '{userId}'.");
        }



    }
}


