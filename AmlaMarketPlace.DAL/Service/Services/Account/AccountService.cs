using System.Linq;
using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.Models.ViewModels.Account;
using AmlaMarketPlace.Models.DTO;
using System.Net.Mail;
using System.Net;
using System.Data;

namespace AmlaMarketPlace.DAL.Service.Services.Account
{
    public class AccountService
    {
        private readonly AmlaMarketPlaceDbContext _context;

        // Initialize the DbContext
        public AccountService(AmlaMarketPlaceDbContext context)
        {
            _context = context;
        }

        public bool DoesUserExists(string email)
        {
            // Checking if the user already exists in the database
            var existingUser = _context.Users.FirstOrDefault(u => u.EmailAddress == email);
            if (existingUser != null)
            {
                return true;
            }

            return false;
        }

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

        public bool AddNewUser(SignUpViewModel signUpViewModel)
        {
            if (DoesUserExists(signUpViewModel.EmailAddress))
            {
                return false;
            }

            // Creating a new user and adding it to the database
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

            _context.Users.Add(newUser); // Adding the user to the Users table
            _context.SaveChanges(); // Saving the changes to the database

            SendVerificationLink(signUpViewModel.EmailAddress);

            return true;
        }

        public UserDTO GetUserByEmail(string email)
        {
            // Fetching the user from the database based on the email
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

            return null; // Return null if no user is found
        }

        public string UpdateUser(UserDTO userDTO)
        {
            // Fetch the existing user by email
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == userDTO.EmailAddress);

            if (user != null)
            {
                // Update the user properties with the values from the UserDTO
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

            return "User not found."; // Return message if user not found
        }

        // Utility method we can say
        public void SendMessageOnMail(string toEmail, string mailSubject, string mailMessage)
        {
            try
            {
                var fromAddress = new MailAddress("rockervc7@gmail.com", "Amla Marketplace");
                var toAddress = new MailAddress(toEmail);
                const string fromAppPassword = "cxml addl yidq sybs"; // not to be touched.
                string subject = mailSubject;
                string body = $"{mailMessage}";

                var smtp = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromAppPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                System.Diagnostics.Debug.WriteLine("Email sending failed: " + ex.Message);
                throw; // Optionally rethrow the exception
            }
        }

        public bool SendVerificationLink(string emailAddress)
        {
            // Generate a unique verification token
            var verificationToken = Guid.NewGuid().ToString();

            // Set token expiration (e.g., 24 hours)
            var tokenExpiration = DateTime.UtcNow.AddHours(24);

            // Save the token and expiration in the database
            var user = GetUserByEmail(emailAddress);
            user.VerificationToken = verificationToken;
            user.TokenExpiration = tokenExpiration;

            // Save changes to the database
            UpdateUser(user);

            // Create the verification link (you should replace "YourAppUrl" with your actual domain)
            var verificationLink = $"https://localhost:44321/Account/VerifyEmail?token={verificationToken}";

            // Send the email
            string mailSubject = "Email Verification";
            string mailMessage = $"Please click the link to verify your email: {verificationLink}";
            SendMessageOnMail(emailAddress, mailSubject, mailMessage);

            return true; // Email sent successfully
        }

        public UserDTO GetUserByToken(string token)
        {
            // Find the user with the provided token
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


    }
}
