using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.DAL.Service.Services.Admin;
using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.DAL.Service.Services.Profile
{
    public class ProfileService
    {
        private readonly AmlaMarketPlaceDbContext _context;
        public ProfileService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }

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

        public string GetUserRoleById(int userRoleId)
        {
            var userRole = _context.UserRoles.FirstOrDefault(r => r.RoleId == userRoleId);

            return userRole != null ? userRole.Role : "user"; // Default to "user" if role not found
        }
    }
}
