using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace HajurKoCarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CarController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
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
        public IActionResult Create(Car obj)
        {
                _db.Cars.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            
        }
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
