using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Account;

namespace AmlaMarketPlace.DAL.Service.IServices.IAccount
{
    public interface IAccountService
    {
        bool DoesUserExists(string email);
        bool IsValidCredentials(SignInViewModel signInViewModel);
        bool AddNewUser(SignUpViewModel signUpViewModel);
        UserDTO GetUserByEmail(string email);
        UserDTO GetUserById(int id);
        string UpdateUser(UserDTO userDTO);
        void SendMessageOnMail(string toEmail, string mailSubject, string mailMessage);
        string CreateNewVerificationTokenWithValidityTime(string email, int TokenValidityTimeInHours);
        bool SendVerificationLink(string emailAddress);
        bool SendResetPasswordVerificationLink(string email);
        UserDTO GetUserByToken(string token);
        bool UpdatePassword(string email, string password);
        bool UpdateIsEmailVerifiedStatus(string email, bool statusValue);
    }
}
