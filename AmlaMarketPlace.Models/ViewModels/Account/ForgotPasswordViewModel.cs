using System.ComponentModel.DataAnnotations;

namespace AmlaMarketPlace.Models.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
