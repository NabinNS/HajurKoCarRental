// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HajurKoCarRental.Data;
using Microsoft.EntityFrameworkCore;
using HajurKoCarRental.Models;

namespace HajurKoCarRental.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public LoginModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Email is not confirmed.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    await CheckUserStatus(user.Id); // call the function
                    await CheckAndUpdateOffers(_db);
                    await CheckUserActive(_userManager);

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            return Page();
        }
        //Added by self NS to check user regular and set last active time
            public async Task CheckUserStatus(string userId)
            {
                var rentals = await _db.RentalRequests
                    .Where(r => r.UserID == userId && r.RequestDate.Month == DateTime.Now.Month)
                    .ToListAsync();
            var user = await _userManager.FindByIdAsync(userId) as ApplicationUser;
            user.LastActive = DateTime.Now;
            //var forpaymentrentaldata = await _db.RentalRequests.Where(r => r.UserID == userId).ToListAsync();
            //bool anyNotPaid = forpaymentrentaldata.Any(r => r.Paid == false);
            //user.PaymentDue = anyNotPaid;

            if (rentals.Count >= 3)
                {
   
                    user.IsRegular = true;

                }
            await _userManager.UpdateAsync(user);
        }


        public static async Task CheckAndUpdateOffers(ApplicationDbContext db)
        {
            try
            {
                var now = DateTime.Now;
                var offersToUpdate = await db.Offers
                    .Where(o => o.Status == true && o.EndDate < now)
                    .ToListAsync();

                foreach (var offer in offersToUpdate)
                {
                    offer.Status = false;
                }

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking and updating offers: {ex.Message}");
            }
        }


        public static async Task CheckUserActive(UserManager<IdentityUser> userManager)
        {
            var users = await userManager.Users.OfType<ApplicationUser>().ToListAsync();

            foreach (var user in users)
            {
                if (user.LastActive.HasValue && (DateTime.Now - user.LastActive.Value).TotalDays >= 90)
                {
                    user.IsActive = false;
               
                }
                await userManager.UpdateAsync(user);
            }
        }


    }
}
