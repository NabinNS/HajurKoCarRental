using HajurKoCarRental.Areas.Identity.Pages.Account;
using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace HajurKoCarRental.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RegisterModel _registerModel;


        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager, RegisterModel registerModel)
        {
            _db = db;
            _userManager = userManager;
            _registerModel = registerModel;
        }
        public IActionResult Index()
        {
           
            IEnumerable<ApplicationUser> userList = _db.Set<ApplicationUser>();

            return View(userList);
        }

        public IActionResult EditUserDetail (string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userFromDb = _db.ApplicationUsers.SingleOrDefault(u => u.Id == id);

            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }
        //Post
        [HttpPost]
        public async Task<IActionResult> EditUserDetail(ApplicationUser user)
        {
                var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == user.Id);

                if (userFromDb == null)
                {
                    return NotFound();
                }

                userFromDb.Name = user.Name;
                userFromDb.Email = user.Email;
                userFromDb.PhoneNumber = user.PhoneNumber;

                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
        //Post
        [HttpPost]

        public async Task<IActionResult> UpdatePassword(string userId, string newPassword, string confirmPassword)
        {


            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (userFromDb == null)
            {
                return NotFound();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "The new password and confirmation password do not match.");
                return RedirectToAction("Index");
            }

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var hashedPassword = passwordHasher.HashPassword(userFromDb, newPassword);
            userFromDb.PasswordHash = hashedPassword;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(string id)
        {
            var obj = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (id == null || obj == null)
            {
                return NotFound();
            }

            _db.ApplicationUsers.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddDocument()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(currentUser);
        }

        [HttpPost]
        public async Task<IActionResult> AddDocumentToDB(IFormFile DrivingLicense, IFormFile Citizenship)
        {
            var user = await _userManager.GetUserAsync(User) as ApplicationUser;
            if ((Citizenship?.Length ?? 0) > 1572864 || (DrivingLicense?.Length ?? 0) > 1572864)
            {
                ModelState.AddModelError(string.Empty, "The size of the  photo must not be more than 1.5 MB.");
                return RedirectToAction("AddDocument");
            }

            user.Verified = Citizenship != null || DrivingLicense != null;

            if (Citizenship != null)
            {
                string citizenshipUrl = await _registerModel.SaveDocumentPhoto(Citizenship, user.Name, "Citizenship");

                user.CitizenshipURL = citizenshipUrl;
            }
            if (DrivingLicense != null)
            {
                //string licenseUrl = await SaveDocumentPhoto(DrivingLicense, user.Name, "License");

                string licenseUrl = await _registerModel.SaveDocumentPhoto(DrivingLicense, user.Name, "License");
                user.DrivingLicenseURL = licenseUrl;
            }
            await _userManager.UpdateAsync(user);


            return RedirectToAction("Index", "Car");
        }




    }
}
