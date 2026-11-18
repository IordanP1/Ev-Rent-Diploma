using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvRent.Data;
using Stripe.Checkout;
using EvRent.Models;

namespace EvRent.Controllers
{
    [Authorize]
    [Route("Payments")]
    public class PaymentsPageController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PaymentsPageController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("StripeCheckout/{id}")]
        public async Task<IActionResult> StripeCheckout(int id)
        {
            var payment = await _db.Payments
                .Include(p => p.Rental)
                .ThenInclude(r => r.Car)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return NotFound();

            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",

                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = (long)(payment.Amount * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Наем на {payment.Rental.Car.Brand} {payment.Rental.Car.Model}"
                    }
                }
            }
        },

                SuccessUrl = domain + $"/Payments/StripeSuccess/{payment.Id}",
                CancelUrl = domain + $"/Payments/StripeCancel/{payment.Id}"
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        [HttpGet("StripeSuccess/{id}")]
        public async Task<IActionResult> StripeSuccess(int id)
        {
            var payment = await _db.Payments
                .Include(p => p.Rental)
                .ThenInclude(r => r.Car)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return NotFound();

            payment.Status = "paid";
            payment.TransactionReference = $"STRIPE-{DateTime.Now.Ticks}";
            payment.Rental.Status = "paid";

            if (payment.Rental.Car != null)
                payment.Rental.Car.Status = "reserved";

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Плащането беше успешно.";

            return RedirectToAction("MyRentals", "Rentals");
        }

        [HttpGet("StripeCancel/{id}")]
        public IActionResult StripeCancel(int id)
        {
            TempData["ErrorMessage"] = "Плащането беше прекъснато.";
            return RedirectToAction("MyRentals", "Rentals");
        }
    }
}