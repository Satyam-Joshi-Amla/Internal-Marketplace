using Microsoft.AspNetCore.Mvc;

namespace AmlaMarketPlace.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Details()
        {
            return View();
        }
    }
}
