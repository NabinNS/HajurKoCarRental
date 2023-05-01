using HajurKoCarRental.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HajurKoCarRental.Controllers
{
    public class DamageController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DamageController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Damage> allDamages = await _db.Damages
                .Include(d => d.Car)
                .Include(d => d.User)
                .ToListAsync();

            return View(allDamages);
        }

    }
}
