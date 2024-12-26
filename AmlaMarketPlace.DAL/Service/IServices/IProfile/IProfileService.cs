using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.DAL.Service.IServices.IProfile
{
    public interface IProfileService
    {
        UserDTO GetUser(int userID); // Retrieves a user's details based on userID
        bool UpdateUser(UserDTO updatedUser); // Updates user details
    }
}
