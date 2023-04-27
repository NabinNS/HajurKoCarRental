﻿using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HajurKoCarRental.Controllers
{
    public class RentalDataController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RentalDataController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;

        }
        [Authorize]
        //public async Task<IActionResult> Index()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var userType = await _userManager.GetRolesAsync(user);
        //    List<RentalRequest> rentalRequests = new List<RentalRequest>();

        //    if(userType.Contains("Admin") || userType.Contains("Staff"))
        //    {
        //        rentalRequests = await _db.RentalRequests
        //           .Include(r => r.User)
        //           .Include(r => r.Car)
        //           .ToListAsync();
        //    }
        //    else if (userType.Contains("Customer"))
        //    {
        //        rentalRequests = await _db.RentalRequests
        //          .Include(r => r.User)
        //          .Include(r => r.Car)
        //          .Where(r => r.User.Id == user.Id)
        //          .ToListAsync();
        //    }
        //    return View(rentalRequests);
        //}
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var userType = await _userManager.GetRolesAsync(user);

            IQueryable<RentalRequest> rentalRequestsQuery = _db.RentalRequests
                .Include(r => r.User)
                .Include(r => r.Car);

            if (userType.Contains("Customer"))
            {
                rentalRequestsQuery = rentalRequestsQuery.Where(r => r.User.Id == user.Id);
            }
            else if (userType.Contains("Admin") || userType.Contains("Staff"))
            {
                rentalRequestsQuery = rentalRequestsQuery.Where(r => r.Status == "Pending");
            }

            var rentalRequests = await rentalRequestsQuery.ToListAsync();

            return View(rentalRequests);
        }



    }
}