using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Profile;

namespace AmlaMarketPlace.BAL.Agent.IAgents.IProfile
{
    public interface IProfileAgent
    {
        ProfileDetailsViewModel GetUser(int userID); // Retrieves user details by user ID
        bool UpdateUser(ProfileDetailsViewModel updatedUser); // Updates user information
    }
}
