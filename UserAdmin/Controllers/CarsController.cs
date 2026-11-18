using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvRent.Models;

namespace EvRent.Controllers
{
    [Route("Cars")]
    public class CarsController : Controller
    {
        private readonly Data.ApplicationDbContext _db;

        public CarsController(Data.ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? category, string? search, int page = 1)
        {
            const int pageSize = 9;

            var query = _db.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(c => c.Category == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Brand.Contains(search) || c.Model.Contains(search));

            int totalCars = await query.CountAsync();

            var cars = await query
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCars / (double)pageSize);
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearch = search;

            return View(cars);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _db.Cars.FindAsync(id);

            if (car == null)
                return NotFound();

            return View(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Car car, IFormFile? imageFile)
        {
            car.Status = string.IsNullOrWhiteSpace(car.Status) ? "available" : car.Status;
            car.Description = string.IsNullOrWhiteSpace(car.Description) ? "Няма описание" : car.Description;
            car.Category = string.IsNullOrWhiteSpace(car.Category) ? "economy" : car.Category;
            car.ConnectorType = string.IsNullOrWhiteSpace(car.ConnectorType) ? "Type 2" : car.ConnectorType;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cars");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                car.ImageUrl = "/images/cars/" + uniqueFileName;
            }
            else
            {
                car.ImageUrl = "/images/no-car.png";
            }


            _db.Cars.Add(car);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _db.Cars.FindAsync(id);

            if (car != null)
            {
                _db.Cars.Remove(car);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _db.Cars.FindAsync(id);

            if (car == null)
                return NotFound();

            return View(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
                return BadRequest();

            car.Status = string.IsNullOrWhiteSpace(car.Status) ? "available" : car.Status;
            car.Description = string.IsNullOrWhiteSpace(car.Description) ? "Няма описание" : car.Description;
            car.Category = string.IsNullOrWhiteSpace(car.Category) ? "economy" : car.Category;
            car.ConnectorType = string.IsNullOrWhiteSpace(car.ConnectorType) ? "Type 2" : car.ConnectorType;
            car.ImageUrl = string.IsNullOrWhiteSpace(car.ImageUrl) ? "/images/no-car.png" : car.ImageUrl;

            _db.Update(car);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
