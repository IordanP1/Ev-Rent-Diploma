using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EvRent.Data;
using EvRent.Models;
using Microsoft.EntityFrameworkCore;

namespace EvRent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public  async Task<IActionResult> Index()
        {
            var cars = await _db.Cars.Where(c => c.Status == "available")
                .Take(4)
                .ToListAsync();
            return View(cars);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
