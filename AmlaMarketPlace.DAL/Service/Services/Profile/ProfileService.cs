using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.DAL.Service.Services.Admin;
using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.DAL.Service.Services.Profile
{
    public class ProfileService
    {
        private readonly AdminService _adminService;
        private readonly AmlaMarketPlaceDbContext _context;
        public ProfileService(AmlaMarketPlaceDbContext context, AdminService adminService)
        { 
            _context = context;
            _adminService = adminService;
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
                    UserRole = _adminService.GetUserRoleById(user.UserRoleId), // Retrieve the user role
                    CreatedOn = user.CreatedOn,
                    EditedOn = user.EditedOn,
                    VerificationToken = user.VerificationToken,
                    TokenExpiration = user.TokenExpiration
                };

                return userDTO;
            }

            return null;
        }
    }
}
