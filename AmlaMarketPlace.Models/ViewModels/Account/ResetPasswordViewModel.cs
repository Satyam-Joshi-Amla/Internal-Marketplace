using System.ComponentModel.DataAnnotations;

namespace AmlaMarketPlace.Models.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$",
    ErrorMessage = "Password must include at least one lowercase letter, one uppercase letter, and one special character and atleast 8 characters long.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmedPassword { get; set; }
    }
}
