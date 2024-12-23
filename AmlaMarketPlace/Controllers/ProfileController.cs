using AmlaMarketPlace.BAL.Agent.IAgents.IProfile;
using AmlaMarketPlace.Models.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace AmlaMarketPlace.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileAgent _profileAgent;
        public ProfileController(IProfileAgent profileAgent)
        {
            _profileAgent = profileAgent;
        }
        public IActionResult Details()
        {
            int id = int.Parse(User.FindFirst("UserId")?.Value);
            ProfileDetailsViewModel user = _profileAgent.GetUser(id);
            ViewData["EnableUserSidePanel"] = true;
            return View(user);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ProfileDetailsViewModel user = _profileAgent.GetUser(id);
            ViewData["EnableUserSidePanel"] = true;
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(ProfileDetailsViewModel user)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _profileAgent.UpdateUser(user);
                if (isUpdated)
                {
                    TempData["UserUpdatedSuccessfully"] = "Successfully updated.";
                }
                else
                {
                    TempData["UserUpdatedFailed"] = "Failed to update.";
                }
                return RedirectToAction("Details");
            }

            return View();
        }
    }
}
