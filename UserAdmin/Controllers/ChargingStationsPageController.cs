using Microsoft.AspNetCore.Mvc;

namespace EvRent.Controllers
{
    public class ChargingStationsPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
