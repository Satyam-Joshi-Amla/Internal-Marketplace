using AmlaMarketPlace.Models.DTO;
using global::AmlaMarketPlace.Models.DTO;
using System;

namespace AmlaMarketPlace.DAL.Service.IServices.IProfile
{
    public interface IProfileService
    {
        UserDTO GetUser(int userID); // Retrieves a user's details based on userID
        bool UpdateUser(UserDTO updatedUser); // Updates user details
    }
}
