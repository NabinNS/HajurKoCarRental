using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HajurKoCarRental.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;


        public UserController(ApplicationDbContext db)
        {
            _db = db;
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
                userFromDb.IsRegular = user.IsRegular;
                userFromDb.IsActive = user.IsActive;

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







    }
}
