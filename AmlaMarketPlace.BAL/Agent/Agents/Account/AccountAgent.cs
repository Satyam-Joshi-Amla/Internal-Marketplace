using AmlaMarketPlace.BAL.Agent.IAgents.IAccount;
using AmlaMarketPlace.DAL.Service.IServices.IAccount;
using AmlaMarketPlace.Models.ViewModels.Account;

namespace AmlaMarketPlace.BAL.Agent.Agents.Account
{
    public class AccountAgent : IAccountAgent
    {
        #region Dependency Injection : Service Fields

        private readonly IAccountService _accountService;

        #endregion

        #region Constructor
        public AccountAgent(IAccountService accountService)
        {
            _accountService = accountService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Calls the account service to add a new user based on the provided sign-up view model.
        /// </summary>
        /// <param name="signUpViewModel">The sign-up data.</param>
        /// <returns>Returns `true` if the user was successfully added, otherwise `false`.</returns>
        public bool SignUp(SignUpViewModel signUpViewModel)
        {
            return _accountService.AddNewUser(signUpViewModel);
        }

        /// <summary>  
        /// Checks if the given email is already registered.  
        /// </summary>  
        /// <param name="email">The email to check.</param>  
        /// <returns>Returns `true` if the email is registered, otherwise `false`.</returns>  
        public bool isEmailRegistered(string email)
        {
            return _accountService.DoesUserExists(email);
        }

        /// <summary>  
        /// Validates the provided sign-in credentials.  
        /// </summary>  
        /// <param name="signInViewModel">The sign-in data.</param>  
        /// <returns>Returns `true` if the credentials are valid, otherwise `false`.</returns>  
        public bool isValidCredentials(SignInViewModel signInViewModel)
        {
            return _accountService.IsValidCredentials(signInViewModel);
        }

        /// <summary>  
        /// Retrieves the user's first name based on the provided email.  
        /// </summary>  
        /// <param name="email">The user's email.</param>  
        /// <returns>Returns the user's first name.</returns>  
        public string getUserNameByEmail(string email)
        {
            var userFirstName = _accountService.GetUserByEmail(email).FirstName;

            return userFirstName;
        }

        /// <summary>  
        /// Retrieves the user's role based on the provided email.  
        /// </summary>  
        /// <param name="email">The user's email.</param>  
        /// <returns>Returns the user's role.</returns>  
        public string getUserRoleByEmail(string email)
        {
            var userRoleValue = _accountService.GetUserByEmail(email).UserRole;

            return userRoleValue;
        }

        /// <summary>  
        /// Retrieves the user ID based on the provided email.  
        /// </summary>  
        /// <param name="email">The user's email.</param>  
        /// <returns>Returns the user's ID.</returns>  
        public int getUserIdByEmail(string email)
        {
            int id = _accountService.GetUserByEmail(email).UserId;

            return id;
        }

        /// <summary>  
        /// Verifies the given token and checks if it is valid and not expired.  
        /// </summary>  
        /// <param name="token">The verification token to check.</param>  
        /// <returns>Returns `true` if the token is valid and not expired, otherwise `false`.</returns>  
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

        //  Do not touch.
        /// <summary>  
        /// Verifies the user's email using the provided token and updates the email verification status. Specifically for verifying the email after sign up.
        /// </summary>  
        /// <param name="token">The verification token for email verification.</param>  
        /// <returns>Returns `true` if email verification is successful.</returns>  
        public bool VerifyEmail(string token)
        {
            bool IsTokenVerified = VerifyTokenWithTime(token);

            string email = _accountService.GetUserByToken(token).EmailAddress;

            // Update the user's email verification status
            _accountService.UpdateIsEmailVerifiedStatus(email, true);

            return true; // Email verification successful
        }

        /// <summary>  
        /// Checks if the given email is already verified.  
        /// </summary>  
        /// <param name="email">The email to check.</param>  
        /// <returns>Returns `true` if the email is verified, otherwise `false`.</returns>  
        public bool isEmailAlreadyVerified(string email)
        {
            if (_accountService.GetUserByEmail(email).IsEmailVerified)
            {
                return true;
            }

            return false;
        }

        /// <summary>  
        /// Updates the user's password based on the provided email and new password model.  
        /// </summary>  
        /// <param name="email">The user's email.</param>  
        /// <param name="resetPasswordViewModel">The model containing the new password.</param>  
        /// <returns>Returns `true` if the password update is successful, otherwise `false`.</returns>  
        public bool UpdatePassword(string email, ResetPasswordViewModel resetPasswordViewModel)
        {
            return _accountService.UpdatePassword(email, resetPasswordViewModel.Password);
        }

        /// <summary>  
        /// Sends a reset password verification link to the provided email address.  
        /// </summary>  
        /// <param name="email">The user's email address.</param>  
        /// <returns>Returns `true` if the link is sent successfully, otherwise `false`.</returns> 
        public bool SendResetPasswordVerificationLink(string email)
        {
            return _accountService.SendResetPasswordVerificationLink(email);
        }

        /// <summary>  
        /// Retrieves the user's email address from the provided token.  
        /// </summary>  
        /// <param name="token">The verification token.</param>  
        /// <returns>Returns the user's email address.</returns>  
        public string GetUserEmailFromToken(string token)
        {
            return _accountService.GetUserByToken(token).EmailAddress;
        }

        /// <summary>  
        /// Sends an email verification link to the specified email address.  
        /// </summary>  
        /// <param name="emailAddress">The email address to send the verification link to.</param>  
        /// <returns>Returns `true` if the link is sent successfully, otherwise `false`.</returns> 
        public bool SendEmailVerificationLink(string emailAddress)
        {
            return _accountService.SendVerificationLink(emailAddress);
        }

        #endregion
    }
}
