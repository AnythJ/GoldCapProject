using GoldCap.Models;
using GoldCap.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "dashboard");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return Json(true);
            return Json($"Email {email} is already taken");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    return RedirectToAction("index", "dashboard");
                }

                ModelState.AddModelError(string.Empty, "Something went wrong, please try again");

            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutPost()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Account");
        }


        public async Task<IActionResult> MyProfile()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            if(user.ProfilePicture != null)
            {
                var base64 = Convert.ToBase64String(user.ProfilePicture);
                var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
                ViewData["ProfileImageSrc"] = imgSrc;
            }
            
            return View("UserSettings", new ApplicationUser { });
        }


        [HttpPost]
        [ActionName("ProfilePicture")]
        public async Task<IActionResult> ProfilePicturePost(IFormFile file)
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                user.ProfilePicture = memoryStream.ToArray();
            }
            await userManager.UpdateAsync(user);

            return RedirectToAction("MyProfile");
        }
    }
}
