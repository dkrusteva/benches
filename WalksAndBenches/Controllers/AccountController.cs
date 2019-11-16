using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalksAndBenches.Identity.Entities;
using WalksAndBenches.Models;
using WalksAndBenches.Identity;

namespace WalksAndBenches.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<BenchUser> _signInManager;
        private readonly UserManager<BenchUser> _userManager;

        public AccountController(ILogger<AccountController> logger, SignInManager<BenchUser> signInManager, UserManager<BenchUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize(Roles = Constants.BenchAdministratorsRole)]
        public async Task<IActionResult> Admin()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            var users = await _userManager.GetUsersInRoleAsync(Constants.BenchRegisteredUsersRole);
            return View(users);
        }

        [Authorize(Roles = Constants.BenchAdministratorsRole)]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [ActionName("Delete")]
        [Authorize(Roles = Constants.BenchAdministratorsRole)]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Admin", "Account");
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "App");
        }

        public IActionResult Login()
        {
            if(this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    loginModel.Username,
                    loginModel.Password,
                    loginModel.RememberMe,
                    false);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Index", "App");
                    }
                }
            }

            ModelState.AddModelError("", "Failed to Login");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "App");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    user = new BenchUser()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.UserName
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        throw new InvalidOperationException("Could not create BenchUser.");
                    }

                    user = await _userManager.FindByNameAsync(model.UserName);
                    await _userManager.AddToRoleAsync(user, Constants.BenchRegisteredUsersRole);
                    ViewBag.UserMessage = "User Created";
                    ModelState.Clear();
                }
            }

            return View();
        }
    }
}
