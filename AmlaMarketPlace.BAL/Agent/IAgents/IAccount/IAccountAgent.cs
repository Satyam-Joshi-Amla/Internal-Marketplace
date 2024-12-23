using AmlaMarketPlace.Models.ViewModels.Account;

namespace AmlaMarketPlace.BAL.Agent.IAgents.IAccount
{
    public interface IAccountAgent
    {
        bool SignUp(SignUpViewModel signUpViewModel); // Registers a new user
        bool isEmailRegistered(string email); // Checks if the email is already registered
        bool isValidCredentials(SignInViewModel signInViewModel); // Validates user credentials
        string getUserNameByEmail(string email); // Retrieves the user's first name by email
        string getUserRoleByEmail(string email); // Retrieves the user's role by email
        int getUserIdByEmail(string email); // Retrieves the user's ID by email
        bool VerifyTokenWithTime(string token); // Verifies the token and checks its expiration
        bool VerifyEmail(string token); // Verifies the email using the token
        bool isEmailAlreadyVerified(string email); // Checks if the email is already verified
        bool UpdatePassword(string email, ResetPasswordViewModel resetPasswordViewModel); // Updates the user's password
        bool SendResetPasswordVerificationLink(string email); // Sends a reset password verification link
        string GetUserEmailFromToken(string token); // Retrieves the user's email address from the token
        bool SendEmailVerificationLink(string emailAddress); // Sends an email verification link
    }
}
