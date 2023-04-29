using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Mail;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;

namespace HajurKoCarRental.Controllers
{
    public class RequestCarController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RequestCarController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;

        }
        public IActionResult Index()
        {
            IEnumerable<Car> CarList = _db.Cars.Where(c => c.IsAvailable == true);
            return View(CarList);

        }

        [Authorize]
        public IActionResult AddRequestCar(int id)
        {
            // Check if the carId is valid
            var car = _db.Cars.FirstOrDefault(c => c.CarID == id);
            if (car == null)
            {
                // Return an error message to the user

                return BadRequest("Invalid CarID");
            }
            //so that other cannot request same car again
            car.IsAvailable = false;
            _db.Cars.Update(car);
     
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Create a new rental request with the given car ID and user ID
            var requestForRent = new RentalRequest
            {
                CarID = id,
                UserID = userId,
                RequestDate = DateTime.Now,
                Status = "Pending",
            };

            // Add the rental request to the database
            _db.RentalRequests.Add(requestForRent);
            _db.SaveChanges();



            return RedirectToAction("Index", "Car");

        }
        //Request handler for Admin and staff to approve or reject request
        public async Task<IActionResult> RequestHandler(int id, string status)
        {
            // Find the rental request by ID
            var rentalRequest = await _db.RentalRequests.Include(r => r.Car).FirstOrDefaultAsync(r => r.ReqID == id);

            if (rentalRequest == null)
            {
                return BadRequest("Invalid Rental Request ID");  // Return an error message to the user
            }
            //current user logged in id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Update the rental request status
            rentalRequest.Status = status;
            rentalRequest.AuthorizedBy = userId;
            if (status == "Rejected")
            {
                rentalRequest.Car.IsAvailable = true;
            }
            _db.RentalRequests.Update(rentalRequest);
            await _db.SaveChangesAsync();

            if (status == "Approved" || status == "Canceled" || status == "Rejected" || status == "Returned")
            {
                //send email to nootify user about their request
                var user = await _userManager.FindByIdAsync(rentalRequest.UserID);
                var applicationUser = (ApplicationUser)user;
                var emailSubject = "Rental Request Status";
                var emailBody = @$"<html>
               <head>
                   <style>
      
                    label {{
                            font-weight: bold;
                                }}
                        </style>
                    </head>
                    <body>
                        <p>Dear {applicationUser.Name},</p>
                 <img src='{rentalRequest.Car.CarImageUrl}' alt='Image description' style='width: 300px; height: 200px; object-fit: cover;'>
                        <p>Your rental request for:</p>
                        <ul>
                            <li>
                                <label>Manufacturer: </label> {rentalRequest.Car.Manufacturer}
                            </li>
                            <li>
                                <label>Model: </label> {rentalRequest.Car.Model}
                            </li>
                            <li>
                                <label>Color: </label> {rentalRequest.Car.Color}
                            </li>
                            <li>
                                <label>Vehicle: </label> {rentalRequest.Car.VehicleNo}
                                  </li>
                             </ul>
                            <p>has been {status}.</p>
                         <p>Thank you,</p>
                        <p>The Car Rental Team</p>
                    </body>
                    </html>";
                await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
            return RedirectToAction("Index", "RentalData");
        }



        //Request handler for customer to either return or cancel request
        public async Task<IActionResult> CustomerRequestHandler(int id, string status)
        {
            // Find the rental request by ID
            var rentalRequest = await _db.RentalRequests.Include(r => r.Car).FirstOrDefaultAsync(r => r.ReqID == id);

            if (rentalRequest == null)
            {
                return BadRequest("Invalid Rental Request ID");  // Return an error message to the user
            }
            //current user logged in id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            rentalRequest.Car.IsAvailable = true; // update car status aftering canceling request
            // Update the rental request status
            rentalRequest.Status = status;
            _db.RentalRequests.Update(rentalRequest);
            await _db.SaveChangesAsync();

            //var staffs = await _userManager.GetUsersInRoleAsync("Staff");
            //foreach (var staff in staffs)
            //{
            //    var subject = "New Offer";
            //    var message = "Dear staffs, A new request has been made.";
            //    await _emailSender.SendEmailAsync(staff.Email, subject, message);
            //}
            return RedirectToAction("Index", "RentalData");
        }


        //Request handler for Admin and staff to  reject request
        public async Task<IActionResult> RequestCancelHandler(int id, string status)
        {
            // Find the rental request by ID
            var rentalRequest = await _db.RentalRequests.Include(r => r.Car).FirstOrDefaultAsync(r => r.ReqID == id);

            if (rentalRequest == null)
            {
                return BadRequest("Invalid Rental Request ID");  // Return an error message to the user
            }
            //current user logged in id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Update the rental request status
            rentalRequest.Status = status;
            rentalRequest.AuthorizedBy = userId;
            _db.RentalRequests.Update(rentalRequest);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "RentalData");
        }


        //handle return request by customer
        [HttpPost]
        public async Task<IActionResult> ReturnCustomerHandler()
        {
            var rentalId = Request.Form["rentalId"];
            int id = int.Parse(rentalId);
            // Find the rental request by ID
            var rentalRequest = await _db.RentalRequests.Include(r => r.Car).FirstOrDefaultAsync(r => r.ReqID == id);

            if (rentalRequest == null)
            {
                return BadRequest("Invalid Rental Request ID");  // Return an error message to the user
            }

            string damage = Request.Form["Damage"];
            string damageDescription = Request.Form["DamageDescription"];
            string damageType = Request.Form["DamageType"];
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Update the rental request status
            rentalRequest.Status = "Return Pending";
            rentalRequest.AuthorizedBy = userId;
            rentalRequest.ReturnDate = DateTime.Now;
            rentalRequest.damage = damage;
            rentalRequest.Car.IsAvailable = true;
            if (damage == "true")
            {
                // Create a new instance of Damage
                var damageRecord = new Damage
                {
                    UserID = userId,
                    CarID = rentalRequest.Car.CarID,
                    RentalID = rentalRequest.ReqID,
                    DamageDescription = damageDescription,
                    DamageType = damageType
                    
                };

                // Add the damage record to the context and save changes
                _db.Damages.Add(damageRecord);
                await _db.SaveChangesAsync();
            }

            // Update the rental request and redirect to RentalData Index
            _db.RentalRequests.Update(rentalRequest);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "RentalData");
        }

        //Return further processing page when admin clicks on return 
        public async Task<IActionResult> ReturnProcessing(int id)
        {
           var rentaldata = await _db.RentalRequests
                .Include(r => r.User)
                .Include(r => r.Car)
                .Include(r => r.Damages) 
                .FirstOrDefaultAsync(r => r.ReqID == id);

            if (rentaldata == null)
            {
                return NotFound();
            }

            return View(rentaldata);
        }
        public async Task<IActionResult> AddBill()
        {
            var rentalId = Request.Form["RequestID"];
            var total = Request.Form["Total"];

            var rentalRequest = await _db.RentalRequests.FindAsync(int.Parse(rentalId));
            rentalRequest.TotalAmount = decimal.Parse(total);
            rentalRequest.Paid = false;
            rentalRequest.Status = "Returned";
            await _db.SaveChangesAsync();


            return RedirectToAction("Index", "RentalData");
        }




    }
}
