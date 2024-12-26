using System.ComponentModel.DataAnnotations;

namespace AmlaMarketPlace.Models.ViewModels.Account
{
    public class SignInViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
