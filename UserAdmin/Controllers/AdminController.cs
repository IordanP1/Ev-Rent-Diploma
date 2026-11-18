using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvRent.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly Data.ApplicationDbContext _db;

        public AdminController(Data.ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCars = await _db.Cars.CountAsync();
            ViewBag.AvailableCars = await _db.Cars.CountAsync(c => c.Status == "available");
            ViewBag.ReservedCars = await _db.Cars.CountAsync(c => c.Status == "reserved");

            ViewBag.TotalRentals = await _db.Rentals.CountAsync();
            ViewBag.ActiveRentals = await _db.Rentals.CountAsync(r => r.Status == "reserved");
            ViewBag.CompletedRentals = await _db.Rentals.CountAsync(r => r.Status == "completed");
            ViewBag.CancelledRentals = await _db.Rentals.CountAsync(r => r.Status == "cancelled");

            ViewBag.TotalRevenue = await _db.Rentals
                .Where(r => r.Status == "completed")
                .SumAsync(r => r.TotalPrice);

            return View();
        }

        [HttpGet("Statistics")]
        public async Task<IActionResult> Statistics()
        {
            ViewBag.AvailableCars = await _db.Cars.CountAsync(c => c.Status == "available");
            ViewBag.ReservedCars = await _db.Cars.CountAsync(c => c.Status == "reserved");

            ViewBag.CompletedRentals = await _db.Rentals.CountAsync(r => r.Status == "completed");
            ViewBag.CancelledRentals = await _db.Rentals.CountAsync(r => r.Status == "cancelled");
            ViewBag.ActiveRentals = await _db.Rentals.CountAsync(r => r.Status == "reserved" || r.Status == "paid");

            ViewBag.TotalRevenue = await _db.Rentals
                .Where(r => r.Status == "completed" || r.Status == "paid")
                .SumAsync(r => r.TotalPrice);

            return View();
        }
    }
}
