using EvRent.Data;
using EvRent.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvRent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChargingStationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ChargingStationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? city, [FromQuery] bool? fastCharging)
        {
            var query = _db.ChargingStations.AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(cs => cs.City == city);
            }

            if (fastCharging.HasValue)
            {
                query = query.Where(s => s.IsFastCharging == fastCharging.Value);
            }
            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var station = await _db.ChargingStations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            return Ok(station);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ChargingStation station)
        {
            _db.ChargingStations.Add(station);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = station.Id }, station);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ChargingStation updatedStation)
        {
            if (id != updatedStation.Id)
            {
                return BadRequest();
            }
            var station = await _db.ChargingStations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            station.Name = updatedStation.Name;
            station.Location = updatedStation.Location;
            station.Latitude = updatedStation.Latitude;
            station.Longitude = updatedStation.Longitude;
            station.Operator = updatedStation.Operator;
            station.PowerOutputKw = updatedStation.PowerOutputKw;
            station.ConnectorType = updatedStation.ConnectorType;
            station.IsFastCharging = updatedStation.IsFastCharging;
            station.Status = updatedStation.Status;
            station.City = updatedStation.City;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var station = await _db.ChargingStations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            _db.ChargingStations.Remove(station);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}