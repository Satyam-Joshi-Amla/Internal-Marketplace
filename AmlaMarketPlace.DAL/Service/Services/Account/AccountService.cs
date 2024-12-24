using System.Net;
using System.Net.Mail;
using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.ViewModels.Account;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.DAL.Service.IServices.IAccount;
using AmlaMarketPlace.ConfigurationManager.UtilityMethods;

namespace AmlaMarketPlace.DAL.Service.Services.Account
{
    public class AccountService : IAccountService
    {

        private readonly AmlaMarketPlaceDbContext _context;
        public AccountService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }


        #region Methods
        /// <summary>
        /// Checks if the email exists in database
        /// </summary>
        /// <param name="email">Email entered by user</param>
        /// <returns>Returns true if email exists else returns false</returns>
        public bool DoesUserExists(string email)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.EmailAddress == email);
            if (existingUser != null)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks if the password entered is correct
        /// </summary>
        /// <param name="signInViewModel">Email and password entered by the user</param>
        /// <returns>Returns true if the entered password is correct else returns false</returns>
        public bool IsValidCredentials(SignInViewModel signInViewModel)
        {
            if (!DoesUserExists(signInViewModel.EmailAddress))
            {
                return false;
            }

            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == signInViewModel.EmailAddress && u.Password == signInViewModel.Password);

            if (user != null)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Creates new user in the database
        /// </summary>
        /// <param name="signUpViewModel">Contains details entered by the user</param>
        /// <returns>Returns true if new user is created successfully else returns false</returns>
        public bool AddNewUser(SignUpViewModel signUpViewModel)
        {
            if (DoesUserExists(signUpViewModel.EmailAddress))
            {
                return false;
            }

            var newUser = new User
            {
                FirstName = signUpViewModel.FirstName,
                LastName = signUpViewModel.LastName,
                EmailAddress = signUpViewModel.EmailAddress,
                IsEmailVerified = false,
                Password = signUpViewModel.Password,
                MobileNumber = signUpViewModel.PhoneNumber,
                UserRoleId = 1, // Default role as normal user
                CreatedOn = DateTime.Now,
                EditedOn = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            SendVerificationLink(signUpViewModel.EmailAddress);

            return true;
        }


        /// <summary>
        /// Fetches user details by email
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <returns>Returns UserDTO containing information of the user if user exists else returns null</returns>
        public UserDTO GetUserByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == email);

            if (user != null)
            {
                var userDTO = new UserDTO
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
                    CreatedOn = user.CreatedOn,
                    EditedOn = user.EditedOn,
                    VerificationToken = user.VerificationToken,
                    TokenExpiration = user.TokenExpiration
                };

                var userRoleData = _context.UserRoles.FirstOrDefault(r => r.RoleId == user.UserRoleId);

                userDTO.UserRole = userRoleData != null ? userRoleData.Role : "user";

                return userDTO;
            }

            return null;
        }


        /// <summary>
        /// Fetches user details by user id
        /// </summary>
        /// <param name="id">User's id</param>
        /// <returns>Returns UserDTO with details of user if exists else returns null</returns>
        public UserDTO GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user != null)
            {
                var userDTO = new UserDTO
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
                    CreatedOn = user.CreatedOn,
                    EditedOn = user.EditedOn,
                    VerificationToken = user.VerificationToken,
                    TokenExpiration = user.TokenExpiration
                };

                var userRoleData = _context.UserRoles.FirstOrDefault(r => r.RoleId == user.UserRoleId);

                userDTO.UserRole = userRoleData != null ? userRoleData.Role : "user";

                return userDTO;
            }

            return null;
        }


        /// <summary>
        /// Updates the user information as per the details in UserDTO
        /// </summary>
        /// <param name="userDTO">Contains details of the user</param>
        /// <returns>Returns string based on success or failure of update</returns>
        public string UpdateUser(UserDTO userDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == userDTO.EmailAddress);
            if (user != null)
            {
                user.FirstName = userDTO.FirstName;
                user.LastName = userDTO.LastName;
                user.IsEmailVerified = userDTO.IsEmailVerified;
                // user.Password = userDTO.Password;
                user.MobileNumber = userDTO.MobileNumber;
                user.IsmobileNumberVerified = userDTO.IsmobileNumberVerified;
                user.UserRoleId = userDTO.UserRoleId;
                user.EditedOn = DateTime.Now;
                user.VerificationToken = userDTO.VerificationToken;
                user.TokenExpiration = userDTO.TokenExpiration;

                _context.Users.Update(user);
                _context.SaveChanges();
                return "User updated successfully.";
            }
            return "User not found.";
        }


        /// <summary>
        /// Shoots an email based on the parameters
        /// </summary>
        /// <param name="toEmail">Email id of the reciever</param>
        /// <param name="mailSubject">String type subject of the email</param>
        /// <param name="mailMessage">String body of the email</param>
        /// <exception cref="Exception"></exception>
        public void SendMessageOnMail(string toEmail, string mailSubject, string mailMessage)
        {
            try
            {
                MailUtility.SendMessageOnMail(toEmail, mailSubject, mailMessage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Email sending failed: " + ex.Message);
                throw new Exception("Something went wrong in account service layer", ex);
            }
        }


        /// <summary>
        /// Creates an unique verification token for verifying the email of the user and saves the token for the user in db
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="TokenValidityTimeInHours">Expiration duration for token in hours</param>
        /// <returns>Returns the unique verification token</returns>
        public string CreateNewVerificationTokenWithValidityTime(string email, int TokenValidityTimeInHours)
        {
            var user = GetUserByEmail(email);
            string newToken = Guid.NewGuid().ToString();
            var newTokenExpiration = DateTime.UtcNow.AddHours(TokenValidityTimeInHours);
            var dbUser = _context.Users.FirstOrDefault(u => u.EmailAddress == email);
            dbUser.VerificationToken = newToken;
            dbUser.TokenExpiration = newTokenExpiration;
            _context.SaveChanges();
            return newToken;
        }


        /// <summary>
        /// Shoots an email to the user with verification link
        /// </summary>
        /// <param name="emailAddress">Email id of the user</param>
        /// <returns>Returns true</returns>
        public bool SendVerificationLink(string emailAddress)
        {
            UserDTO user = GetUserByEmail(emailAddress);
            string verificationToken = CreateNewVerificationTokenWithValidityTime(emailAddress, 24);
            var verificationLink = $"https://localhost:44321/Account/VerifyEmail?token={verificationToken}";
            string mailSubject = "Email Verification";
            string mailMessage = $@"
            Hi {user.FirstName} {user.LastName},
            Please click the link below to verify your email address:

            {verificationLink}

            Thank you for joining us!

            Best regards,
            Amla Marketplace Team";

            SendMessageOnMail(emailAddress, mailSubject, mailMessage);
            return true;
        }


        /// <summary>
        /// Sends password reset link to the user on email id
        /// </summary>
        /// <param name="email">Email id of the user</param>
        /// <returns>Returns true</returns>
        public bool SendResetPasswordVerificationLink(string email)
        {
            string verificationToken = CreateNewVerificationTokenWithValidityTime(email, 24);
            var verificationLink = $"https://localhost:44321/Account/VerifyAndRedirectToResetPassword?token={verificationToken}";
            UserDTO user = GetUserByEmail(email);
            string mailSubject = "Email Verification for Reset Password";
            string mailMessage = $@"
             Hi {user.FirstName} {user.LastName},

             Please click the link below to verify.:

             {verificationLink}

             Thank you for joining us!

             Best regards,  
             Amla Marketplace Team";

            SendMessageOnMail(user.EmailAddress, mailSubject, mailMessage);
            return true;
        }


        /// <summary>
        /// Fetches user details with reference to token string
        /// </summary>
        /// <param name="token">Unique token string sent to the user</param>
        /// <returns>Returns UserDTO type user details if user exists else returns null</returns>
        public UserDTO GetUserByToken(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.VerificationToken == token);

            if (user != null)
            {
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    IsEmailVerified = user.IsEmailVerified,
                    //Password = user.Password,
                    MobileNumber = user.MobileNumber,
                    IsmobileNumberVerified = user.IsmobileNumberVerified,
                    UserRoleId = user.UserRoleId,
                    EditedOn = DateTime.Now,
                    VerificationToken = user.VerificationToken,
                    TokenExpiration = user.TokenExpiration
                };
                return userDTO;
            }
            return null;
        }


        /// <summary>
        /// Updates the password of the user
        /// </summary>
        /// <param name="email">Email id of the user</param>
        /// <param name="password">New password set by the user</param>
        /// <returns>Returns true if the password is updated else returns false</returns>
        public bool UpdatePassword(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == email);

            if (user == null || string.IsNullOrEmpty(password))
            {
                return false;
            }
            else
            {
                user.Password = password;
                _context.SaveChanges();
                return true;
            }
        }


        /// <summary>
        /// Updates the status of email verification
        /// </summary>
        /// <param name="email">Email id of the user</param>
        /// <param name="statusValue">Status of the verification</param>
        /// <returns>Returns true if the status is updated else returns false</returns>
        public bool UpdateIsEmailVerifiedStatus(string email, bool statusValue)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == email);

            if (user != null)
            {
                user.IsEmailVerified = statusValue;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion
    }
}
