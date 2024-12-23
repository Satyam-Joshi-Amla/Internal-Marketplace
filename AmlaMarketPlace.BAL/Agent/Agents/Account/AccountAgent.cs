using AmlaMarketPlace.Models.ViewModels.Account;
using AmlaMarketPlace.BAL.Agent.IAgents.IAccount;
using AmlaMarketPlace.DAL.Service.IServices.IAccount;

namespace AmlaMarketPlace.BAL.Agent.Agents.Account
{
    public class AccountAgent : IAccountAgent
    {
        private readonly IAccountService _accountService;
        public AccountAgent(IAccountService accountService)
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

        public string getUserRoleByEmail(string email)
        {
            var userRoleValue = _accountService.GetUserByEmail(email).UserRole;

            return userRoleValue;
        }

        public int getUserIdByEmail(string email)
        {
            int id = _accountService.GetUserByEmail(email).UserId;

            return id;
        }

        public bool VerifyTokenWithTime(string token)
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

            return true;
        }

        // Specifically for verifying the email after sign up. Do not touch.
        public bool VerifyEmail(string token)
        {
            bool IsTokenVerified = VerifyTokenWithTime(token);

            string email = _accountService.GetUserByToken(token).EmailAddress;

            // Update the user's email verification status
            _accountService.UpdateIsEmailVerifiedStatus(email, true);

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

        public bool UpdatePassword(string email, ResetPasswordViewModel resetPasswordViewModel)
        {
            return _accountService.UpdatePassword(email, resetPasswordViewModel.Password);
        }

        public bool SendResetPasswordVerificationLink(string email)
        {
            return _accountService.SendResetPasswordVerificationLink(email);            
        }

        public string GetUserEmailFromToken(string token)
        {
            return _accountService.GetUserByToken(token).EmailAddress;
        }

        public bool SendEmailVerificationLink(string emailAddress)
        {
            return _accountService.SendVerificationLink(emailAddress);
        }
    }
}
