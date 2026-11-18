using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvRent.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EvRent.Controllers
{
    [Authorize]
    [Route("Rentals")]
    public class RentalsController : Controller
    {
        private readonly Data.ApplicationDbContext _db;

        public RentalsController(Data.ApplicationDbContext db)
        {
            _db = db;
        }


        [HttpGet("Create")]
        public async Task<IActionResult> Create(int carId)
        {
            var car = await _db.Cars.FindAsync(carId);

            if (car == null)
                return NotFound();

            var rental = new Rental
            {
                CarId = car.Id,
                Car = car,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                PaymentMethod = "cash",
                Status = "pending"
            };

            return View(rental);
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(Rental rental)
        {
            rental.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var car = await _db.Cars.FindAsync(rental.CarId);

            if (car == null)
                return NotFound();

            if (car.Status != "available")
            {
                ModelState.AddModelError("", "Този автомобил вече не е наличен.");
                rental.Car = car;
                return View(rental);
            }

            if (rental.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(rental.StartDate), "Началната дата не може да бъде в миналото."); 
                rental.Car = car;
                return View(rental);
            }

            if (rental.EndDate.Date <= rental.StartDate.Date)
            {
                ModelState.AddModelError(nameof(rental.EndDate), "Крайната дата трябва да бъде след началната.");
                rental.Car = car;
                return View(rental);
            }

            rental.Car = car;

            rental.TotalDays = Math.Max(1, (rental.EndDate - rental.StartDate).Days);
            rental.BasePrice = rental.TotalDays * car.PricePerDay;
            rental.InsurancePrice = rental.BasePrice * 0.08m;
            rental.TotalPrice = rental.BasePrice + rental.InsurancePrice;

            rental.Notes = string.IsNullOrWhiteSpace(rental.Notes) ? "Няма бележки" : rental.Notes;

            rental.Status = rental.PaymentMethod == "online" ? "pending_payment" : "reserved";

            _db.Rentals.Add(rental);

            car.Status = "reserved";

            await _db.SaveChangesAsync();

       
            var payment = new Payment
            {
                RentalId = rental.Id,
                Amount = rental.TotalPrice,
                PaymentMethod = rental.PaymentMethod,
                Status = rental.PaymentMethod == "online" ? "pending" : "pay_on_pickup",
                CustomerEmail = rental.CustomerEmail,
                TransactionReference = $"TXN-{DateTime.Now.Ticks}",
                CreateDate = DateTime.Now
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            
            if (rental.PaymentMethod == "online")
            {
                return RedirectToAction("StripeCheckout", "PaymentsPage", new { id = payment.Id });
            }

            
            return RedirectToAction("Details", "Cars", new { id = car.Id });
        }

        [HttpGet("MyRentals")]
        public async Task<IActionResult> MyRentals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _db.Rentals
                .Include(r => r.Car)
                .OrderByDescending(r => r.StartDate)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(r => r.UserId == userId);
            }

            var rentals = await query.ToListAsync();

            foreach (var rental in rentals)
            {
                if ((rental.Status == "reserved" || rental.Status == "paid") &&
                    rental.EndDate.Date < DateTime.Today)
                {
                    rental.Status = "completed";

                    if (rental.Car != null)
                    {
                        rental.Car.Status = "available";
                    }
                }
            }

            await _db.SaveChangesAsync();

            return View(rentals);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var rental = await _db.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
                return NotFound();

            rental.Status = "completed";

            if (rental.Car != null)
            {
                rental.Car.Status = "available";
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Наемът беше приключен успешно.";

            return RedirectToAction(nameof(MyRentals));
        }
        [HttpPost("Cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var rental = await _db.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
                return NotFound();

            rental.Status = "cancelled";

            if (rental.Car != null)
            {
                rental.Car.Status = "available";
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("Admin") && rental.UserId != userId)
            {
                return Forbid();
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Резервацията беше отказана успешно.";

            return RedirectToAction(nameof(MyRentals));
        }
    }
}