using AmlaMarketPlace.Models.ViewModels.Account;
using AmlaMarketPlace.DAL.Service.Services.Account;
using System.Net.Mail;
using System.Net;
using AmlaMarketPlace.Models.DTO;

namespace AmlaMarketPlace.BAL.Agent.Agents.Account
{
    public class AccountAgent
    {
        private readonly AccountService _accountService;
        public AccountAgent(AccountService accountService)
        {
            _accountService = accountService;
        }

        public bool SignUp(SignUpViewModel signUpViewModel)
        {
            return _accountService.AddNewUser(signUpViewModel);
        }

        public bool isEmailRegistered(string email)
        {
            return _accountService.DoesUserExists(email);
        }

        public bool isValidCredentials(SignInViewModel signInViewModel)
        {
            return _accountService.IsValidCredentials(signInViewModel);
        }

        public string getUserNameByEmail(string email)
        {
            var userFirstName = _accountService.GetUserByEmail(email).FirstName;

            return userFirstName;
        }

        public int getUserIdByEmail(string email)
        {
            int id = _accountService.GetUserByEmail(email).UserId;

            return id;
        }

        // Specifically for verifying the email after clicking on the link. Do not touch.
        public bool VerifyEmail(string token)
        {
            // Find the user by the verification token
            var userDTO = _accountService.GetUserByToken(token);

            if (userDTO == null)
            {
                // Invalid or expired token
                return false;
            }

            // Check if the token is expired
            if (userDTO.TokenExpiration < DateTime.UtcNow)
            {
                // Token has expired
                return false;
            }

            // Update the user's email verification status
            userDTO.IsEmailVerified = true;
            //userDTO.VerificationToken = null; // Clear the token after successful verification
            //userDTO.TokenExpiration = null; // Clear the expiration date

            // Update user in the database
            _accountService.UpdateUser(userDTO);

            return true; // Email verification successful
        }

        public bool isEmailAlreadyVerified(string email)
        {
            if (_accountService.GetUserByEmail(email).IsEmailVerified)
            {
                return true;
            }

            return false;
        }

    }
}
