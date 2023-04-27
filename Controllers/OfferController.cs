using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HajurKoCarRental.Controllers
{
    public class OfferController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
       
        public OfferController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var offerlist = _db.Offers.Include(o => o.Car).ToList();
            //IEnumerable<Offer> offerlist = _db.Offers;
            return View(offerlist);
        }
        public async Task<ActionResult> CreateOffer()
        {
            //var cars = await _db.Cars.Select(c => new { ID = c.CarID, Name = c.Model + "-" + c.Color }).ToListAsync();
            //ViewBag.CarID = new SelectList(cars, "ID", "Name");
            //return View();
            IEnumerable<Car> CarList = _db.Cars;
            return View(CarList);
        }

        //Post
        [HttpPost]
        public async Task<ActionResult> AddOffer()
        {
            string carId = Request.Form["carId"];
            string discountRate = Request.Form["DiscountRate"];
            string endDate = Request.Form["EndDate"];
            string offerDescription = Request.Form["OfferDescription"];

            // create a new instance of the Offer class and populate its properties
            //if (!int.TryParse(carId, out int carIdInt) || !decimal.TryParse(discountRate, out decimal discountRateDecimal))
            //{
            //    // handle the error here
            //    return BadRequest();
            //}
            Offer offer = new Offer()
            {
                CarID = int.Parse(carId),
                DiscountRate = decimal.Parse(discountRate),
                EndDate = DateTime.Parse(endDate),
                OfferDescription = offerDescription,
                Status = true
            };

            // add the offer to the database
            _db.Offers.Add(offer);
            _db.SaveChanges();

            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            var car = await _db.Cars.FindAsync(int.Parse(carId));
            foreach(var customer in customers)
            {
                var applicationUser = customer as ApplicationUser;
                var subject = "New Offer";
                var message = $@"
            <html>
                <body>
                    <p>Dear {applicationUser.Name},</p>
                    <p>A new offer is now available for the following car:</p>
                  <img src='{car.CarImageUrl}' alt='Image description' style='width: 300px; height: 200px; object-fit: cover;'>
                    <p><strong>{car.Manufacturer} {car.Model} ({car.Color})</strong></p>
                    <p>Offer Details: {offer.OfferDescription}</p>
                    <p>Thank you for using our services!</p>
                </body>
            </html>";
                await _emailSender.SendEmailAsync(customer.Email, subject, message);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            var obj = _db.Offers.Find(id);
            if (id == null || id == 0)
            {
                return NotFound();
            }
            if (obj == null)
            {
                return NotFound();
            }
            _db.Offers.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult CloseOffer(int? id)
        {
            var offerData = _db.Offers.Find(id);
            if (id == null || id == 0)
            {
                return NotFound();
            }
            if (offerData == null)
            {
                return NotFound();
            }
            offerData.Status = false; // Update the offer status to false
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
