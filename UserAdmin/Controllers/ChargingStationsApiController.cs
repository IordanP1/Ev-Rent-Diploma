using Microsoft.AspNetCore.Mvc;
using EvRent.Services;

namespace EvRent.Controllers
{
    [ApiController]
    [Route("api/stations")]
    public class ChargingStationsApiController : ControllerBase
    {
        private readonly ChargingStationService _service;

        public ChargingStationsApiController(ChargingStationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var json = await _service.GetStationsAsync();
            return Content(json, "application/json");
        }
    }
}