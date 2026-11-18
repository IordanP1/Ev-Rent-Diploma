using EvRent.Models;
using EvRent.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvRent.Controllers
{
    [Route("Range")]
    public class RangeController : Controller
    {
        private readonly RangeCalculatorService _rangeCalculator;

        public RangeController(RangeCalculatorService rangeCalculator)
        {
            _rangeCalculator = rangeCalculator;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("")]
         public IActionResult Index(
         double batteryCapacityKwh,
         int batteryLevelPercent,
         double averageConsumption,
         string drivingMode,
         string load,
         string conditionMode)
        {
            if (batteryCapacityKwh <= 0)
            {
                ViewBag.Error = "Капацитетът на батерията трябва да бъде по-голям от 0.";
                return View();
            }

            if (batteryLevelPercent < 0 || batteryLevelPercent > 100)
            {
                ViewBag.Error = "Зарядът на батерията трябва да бъде между 0% и 100%.";
                return View();
            }

            if (averageConsumption <= 0)
            {
                ViewBag.Error = "Средният разход трябва да бъде по-голям от 0.";
                return View();
            }

            string climate = "off";
            string temperature = "mild";

            if (conditionMode == "climate")
            {
                climate = "ac";
                temperature = "hot";
            }
            else if (conditionMode == "winter")
            {
                climate = "heating";
                temperature = "cold";
            }

            var request = new RangeRequest
            {
                BatteryCapacityKwh = batteryCapacityKwh,
                BatteryLevelPercent = batteryLevelPercent,
                ConsumptionKwhPer100km = averageConsumption,
                DrivingMode = drivingMode,
                Load = load,
                Climate = climate,
                Temperature = temperature
            };

            var result = _rangeCalculator.Calculate(request);

            ViewBag.Result = result.FinalRangeKm;
            ViewBag.BaseRange = result.BaseRangeKm;
            ViewBag.SafeRange = result.SafeRangeKm;

            return View();
        }
    }
}