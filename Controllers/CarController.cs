using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace HajurKoCarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CarController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(string filter)
        {
            IEnumerable<Car> objCarList;

            switch (filter)
            {
                case "all":
                    objCarList = await _db.Cars.ToListAsync();
                    break;
                case "available":
                    objCarList = await _db.Cars
                    .Include(c => c.Offers)
                    .Where(c => c.IsAvailable)
                    .ToListAsync();
                    break;
                case "rent":
                    objCarList = await _db.Cars.Where(c => !c.IsAvailable).ToListAsync();
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
                    objCarList = await _db.Cars.Where(c => c.IsAvailable).ToListAsync();
                    break;
            }

            return View(objCarList);
        }

        public IActionResult TableCarView()
        {
            IEnumerable<Car> objCarList = _db.Cars;
            return View(objCarList);
        }
        public IActionResult Create()
        {
            return View();
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car obj, IFormFile file)
        {
            var account = new Account(
                "NabinNs",
                "428542427551857",
                "A1gFD-djrOGlzEhYe-ysX_A2JUo");
            var cloudinary = new Cloudinary(account);

            var uploadResult = new ImageUploadResult();
            if (file != null && file.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                };

               uploadResult = await cloudinary.UploadAsync(uploadParams);

            }

            var car = new Car
            {
                Manufacturer = obj.Manufacturer,
                Model = obj.Model,
                RentalRate = obj.RentalRate,
                VehicleNo = obj.VehicleNo,
                IsAvailable = obj.IsAvailable,
                Color = obj.Color,
                CarImageUrl = uploadResult.SecureUrl?.ToString()
            };
            _db.Cars.Add(car);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //_db.Cars.Add(obj);
        //_db.SaveChanges();
        //return RedirectToAction("Index");


        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            var carFromDb = _db.Cars.Find(id);
            if (carFromDb == null)
            {
                return NotFound();
            }
            return View(carFromDb);
        }
        //Post
        [HttpPost]
        public IActionResult Edit(Car car)
        {

            var carFromDb = _db.Cars.Find(car.CarID); // Find the existing car object from the database
            if (carFromDb != null)
            {
                carFromDb.Manufacturer = car.Manufacturer; // Update the car properties with the new values
                carFromDb.Model = car.Model;
                carFromDb.Color = car.Color;
                carFromDb.RentalRate = car.RentalRate;
                carFromDb.VehicleNo = car.VehicleNo;
                carFromDb.IsAvailable = car.IsAvailable;
                carFromDb.CarImageUrl = car.CarImageUrl;

                _db.Cars.Update(carFromDb); // Update the car object in the database
                _db.SaveChanges();

                return RedirectToAction("Index"); // Redirect to the index action
            }

            return NotFound(); // Car
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
