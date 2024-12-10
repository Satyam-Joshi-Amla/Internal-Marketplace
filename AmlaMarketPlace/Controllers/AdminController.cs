using AmlaMarketPlace.BAL.Agent.Agents.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmlaMarketPlace.Controllers
{
    [Authorize()]
    public class AdminController : Controller
    {
        private readonly AdminAgent _adminAgent;
        public AdminController(AdminAgent adminAgent)
        {
            _adminAgent = adminAgent;
        }

        public IActionResult DashBoard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllUsersList()
        {
            var allUsers = _adminAgent.GetAllUsers();
            return View(allUsers);
        }

        [HttpGet]
        public IActionResult GetAllProductsList()
        {
            var allPublishedProducts = _adminAgent.GetAllPublishedProducts();
            return View(allPublishedProducts);
        }
    }
}