using AmlaMarketPlace.DAL.Service.Services.Profile;
using AmlaMarketPlace.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.BAL.Agent.Agents.Profile
{
    public class ProfileAgent
    {
        private readonly ProfileService _profileService;
        public ProfileAgent(ProfileService profileService)
        {
            _profileService = profileService;
        }

        public UserDTO GetUser(int userID)
        {
            return _profileService.GetUser(userID);
        }
    }
}
