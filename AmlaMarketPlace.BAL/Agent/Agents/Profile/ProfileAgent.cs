using AmlaMarketPlace.BAL.Agent.IAgents.IProfile;
using AmlaMarketPlace.DAL.Service.IServices.IProfile;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Profile;

namespace AmlaMarketPlace.BAL.Agent.Agents.Profile
{
    public class ProfileAgent : IProfileAgent
    {
        #region Dependency Injection : Service Fields

        private readonly IProfileService _profileService;

        #endregion

        #region Constructor
        public ProfileAgent(IProfileService profileService)
        {
            _profileService = profileService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the profile details of a user based on the provided userID. It fetches the user's personal information such as name, email, and mobile number from the _profileService.
        /// </summary>
        /// <param name="userID">The ID of the user whose profile details are to be fetched.</param>
        /// <returns>
        /// A ProfileDetailsViewModel containing the user's profile information.
        /// </returns>
        public ProfileDetailsViewModel GetUser(int userID)
        {
            UserDTO user = _profileService.GetUser(userID);
            ProfileDetailsViewModel profileDetails = new ProfileDetailsViewModel()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                IsEmailVerified = user.IsEmailVerified,
                MobileNumber = user.MobileNumber
            };

            return profileDetails;
        }

        /// <summary>
        /// Updates the profile details of a user based on the provided updatedUser information. It maps the ProfileDetailsViewModel to a UserDTO and calls _profileService to save the changes.
        /// </summary>
        /// <param name="updatedUser">The model containing the updated user profile details.</param>
        /// <returns>
        /// A boolean indicating whether the user's profile was successfully updated.
        /// </returns>
        public bool UpdateUser(ProfileDetailsViewModel updatedUser)
        {
            UserDTO updatedUserDTO = new UserDTO()
            {
                UserId = updatedUser.UserId,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                EmailAddress = updatedUser.EmailAddress,
                IsEmailVerified = updatedUser.IsEmailVerified,
                MobileNumber = updatedUser.MobileNumber
            };

            return _profileService.UpdateUser(updatedUserDTO);
        }

        #endregion
    }
}
