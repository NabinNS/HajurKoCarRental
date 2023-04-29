using HajurKoCarRental.Data;
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
                rentalRequestsQuery = rentalRequestsQuery.Where(r => r.User.Id == user.Id && (r.Status == "Approved" || r.Status == "Pending"));
            }
            else if (userType.Contains("Admin") || userType.Contains("Staff"))
            {
                rentalRequestsQuery = rentalRequestsQuery.Where(r => r.Status == "Pending" || r.Status == "Canceled Pending" || r.Status == "Return Pending");
            }

            var rentalRequests = await rentalRequestsQuery.ToListAsync();

            return View(rentalRequests);
        }
        [Authorize]
        public async Task<IActionResult> RentalHistory(DateTime? startDate, DateTime? endDate)
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
            if (startDate != null && endDate != null)
            {
                //filter according to date match of request date
                rentalRequestsQuery = rentalRequestsQuery.Where(r => r.RequestDate >= startDate && r.RequestDate <= endDate);
            }

            if (userType.Contains("Customer"))
            {
                rentalRequestsQuery = rentalRequestsQuery.Where(r => r.User.Id == user.Id);
            }

            var rentalRequests = await rentalRequestsQuery.ToListAsync();

            return View(rentalRequests);

    
        }

    }
}
