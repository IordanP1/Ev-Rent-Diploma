using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvRent.Data;
using EvRent.Models;

namespace EvRent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController:ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PaymentsController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        => Ok(await _db.Payments.Include(p => p.Rental).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var payment = await _db.Payments.Include(p => p.Rental).FirstOrDefaultAsync(p => p.Id == id);
            return payment== null ? NotFound() : Ok(payment);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            payment.CreateDate = DateTime.Now;
            payment.TransactionReference = $"TXN-{DateTime.Now.Ticks}";
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = payment.Id }, payment);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            payment.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
