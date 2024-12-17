using AmlaMarketPlace.BAL.Agent.Agents.Profile;
using Microsoft.AspNetCore.Mvc;

namespace AmlaMarketPlace.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileAgent _profileAgent;
        public ProfileController(ProfileAgent profileAgent)
        {
            _profileAgent = profileAgent;
        }
        public IActionResult Details(int id)
        {
            var user = _profileAgent.GetUser(id);
            return View(user);
        }
    }
}
