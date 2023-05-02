using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;

namespace HajurKoCarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;


        public CarController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string filter)
        {
            IEnumerable<Car> objCarList;
            if (!User.Identity.IsAuthenticated) //to show all availabe cars when no user is logged in
            {
                filter = "all";
            }


            switch (filter) //these are filters created so that admin can car according to filter
            {
                case "all":
                    objCarList = await _db.Cars.Include(c => c.Offers).ToListAsync();
                    break;
                case "available":
                    objCarList = await _db.Cars
                    .Include(c => c.Offers)
                    .Where(c => c.IsAvailable)
                    .ToListAsync();
                    break;
                case "rent":
                    objCarList = await _db.Cars.Where(c => !c.IsAvailable).Include(c => c.Offers).ToListAsync();
                    break;
                case "frequent":
                    objCarList = await _db.RentalRequests
                        .GroupBy(r => r.CarID)
                        .Where(g => g.Count() > 3)
                        .Select(g => g.FirstOrDefault().Car)
                        .ToListAsync();
                    break;
                case "notrented":
                    objCarList = await _db.Cars
                        .Where(c => !c.RentalRequests.Any())
                        .ToListAsync();
                    break;
                default:
                    objCarList = await _db.Cars
                     .Include(c => c.Offers)
                     .Where(c => c.IsAvailable)
                     .ToListAsync();
                    break;
            }

            return View(objCarList);
        }

        public IActionResult TableCarView() //to show cars in tabluar view
        {
            IEnumerable<Car> objCarList = _db.Cars;
            return View(objCarList);
        }
        public IActionResult Create() //return car form view page
        {
            return View();
        }


        private async Task<string?> UploadImageToCloudinaryAsync(IFormFile? file, Cloudinary cloudinary) //add image to cloudinary 
        {
            if (file != null && file.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }

            return null;
        }


        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car obj, IFormFile? file)
        {
            var account = new Account(
                "nabinns",
                "691224812487589",
                "LJXSeT5OvbcJSxaCDfWHblcQ7-Y");
            var cloudinary = new Cloudinary(account); //using cloudinary to add image to cloud

            var imageUrl = await UploadImageToCloudinaryAsync(file, cloudinary);

            if (ModelState.IsValid) //checks if all value properties are correct or not
            {
                var car = new Car
                {
                    Manufacturer = obj.Manufacturer,
                    Model = obj.Model,
                    RentalRate = obj.RentalRate,
                    VehicleNo = obj.VehicleNo,
                    IsAvailable = obj.IsAvailable,
                    Color = obj.Color,
                    CarImageUrl = imageUrl
                };
                _db.Cars.Add(car);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "New car added successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(obj);
            }
        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var carFromDb = _db.Cars.Find(id);
            if (carFromDb == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index"); 
            }
            return View(carFromDb);
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> EditAsync(Car car, IFormFile? file)
        {
            try
            {
                var account = new Account(
                    "nabinns",
                    "691224812487589",
                    "LJXSeT5OvbcJSxaCDfWHblcQ7-Y");
                var cloudinary = new Cloudinary(account); //using cloudinary to add image to cloud

                var carFromDb = _db.Cars.Find(car.CarID); // Find the existing car object from the database
                if (ModelState.IsValid)
                {
                    if (carFromDb != null)
                    {
                        carFromDb.Manufacturer = car.Manufacturer; // Update the car properties with the new values
                        carFromDb.Model = car.Model;
                        carFromDb.Color = car.Color;
                        carFromDb.RentalRate = car.RentalRate;
                        carFromDb.VehicleNo = car.VehicleNo;
                        carFromDb.IsAvailable = car.IsAvailable;

                        var imageUrl = await UploadImageToCloudinaryAsync(file, cloudinary);
                        if (imageUrl != null)
                        {
                            carFromDb.CarImageUrl = imageUrl;
                        }

                        _db.Cars.Update(carFromDb); // Update the car object in the database
                        await _db.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Car data edited successfully";

                        return RedirectToAction("Index"); // Redirect to the index action
                    }
                }
                else
                {
                    return View(car);
                }

                return NotFound(); // Car not found
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while editing the car data: " + ex.Message;
                return RedirectToAction("Index"); // Redirect to the index action with error message
            }
        }


        public IActionResult Delete(int? id)
        {
            var obj = _db.Cars.Find(id);
            if (id == null || id == 0)
            {
                return NotFound();
            }
            if (obj == null)
            {
                return NotFound();
            }
            _db.Cars.Remove(obj);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Car deleted successfully";
            return RedirectToAction("Index");
        }

        public IActionResult ViewCar(int? id)
        {
            var carFromDb = _db.Cars.Find(id);
            if (carFromDb == null)
            {
                return NotFound();
            }

            return View(carFromDb);
        }


    }
}
