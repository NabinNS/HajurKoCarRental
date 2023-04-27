using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HajurKoCarRental.Controllers
{
    public class RequestCarController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RequestCarController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Car> CarList = _db.Cars.Where(c => c.IsAvailable);
            return View(CarList);
        }
        
        [Authorize]
        public IActionResult AddRequestCar(RentalRequest rental, int carId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Create a new rental request with the given car ID and user ID
            var rentalRequest = new RentalRequest
            {
                CarID = carId,
                UserID = userId,
                RequestDate = DateTime.Now,
                ReturnDate = DateTime.Now,
                Status = "Pending",
                AuthorizedBy = userId

            };

            // Add the rental request to the database
            _db.RentalRequests.Add(rentalRequest);
            _db.SaveChanges();


            return RedirectToAction("Index");

        }
    }
}
