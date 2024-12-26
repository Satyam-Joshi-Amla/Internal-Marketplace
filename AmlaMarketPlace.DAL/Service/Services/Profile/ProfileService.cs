using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.DAL.Service.IServices.IProfile;
using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.DAL.Service.Services.Profile
{
    public class ProfileService : IProfileService
    {
        #region Dependency Injection : Database Fields
        private readonly AmlaMarketPlaceDbContext _context;
        #endregion

        #region Constructor
        public ProfileService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }
        #endregion

        #region User Services

        /// <summary>
        /// Retrieves the user details based on the given user ID.
        /// </summary>
        /// <param name="userID">The ID of the user to retrieve.</param>
        /// <returns>A `UserDTO` object containing user details, or `null` if the user is not found.</returns>
        public UserDTO GetUser(int userID)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userID);

            if (user != null)
            {
                UserDTO userDTO = new UserDTO()
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    IsEmailVerified = user.IsEmailVerified,
                    Password = user.Password,
                    MobileNumber = user.MobileNumber,
                    IsmobileNumberVerified = user.IsmobileNumberVerified,
                    UserRoleId = user.UserRoleId,
                    UserRole = GetUserRoleById(user.UserRoleId), // Retrieve the user role
                    CreatedOn = user.CreatedOn,
                    EditedOn = user.EditedOn,
                    VerificationToken = user.VerificationToken,
                    TokenExpiration = user.TokenExpiration
                };

                return userDTO;
            }

            return null;
        }

        /// <summary>
        /// Updates the user information based on the provided UserDTO.
        /// </summary>
        /// <param name="updatedUser">The user data transfer object containing updated user information.</param>
        /// <returns> true if the user is updated successfully, otherwise false.</returns>
        public bool UpdateUser(UserDTO updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);

            if (user != null)
            {

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.MobileNumber = updatedUser.MobileNumber;
                user.EditedOn = DateTime.Now;

                _context.Users.Update(user);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves the user role based on the provided role ID.
        /// </summary>
        /// <param name="userRoleId">The ID of the user role.</param>
        /// <returns> The role name if found, otherwise "user" as the default role.</returns>
        public string GetUserRoleById(int userRoleId)
        {
            var userRole = _context.UserRoles.FirstOrDefault(r => r.RoleId == userRoleId);

            return userRole != null ? userRole.Role : "user"; // Default to "user" if role not found
        }

        #endregion
    }
}
