using System.ComponentModel.DataAnnotations;
using AmlaMarketPlace.ConfigurationManager.CustomValidation;

namespace AmlaMarketPlace.Models.ViewModels.Account
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "First Name can only contain letters and spaces.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Last Name can only contain letters and spaces.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        //[CustomPasswordValidation]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$",
    ErrorMessage = "Password must include at least one lowercase letter, one uppercase letter, and one special character and atleast 8 characters long.")]

        [Display(Name = "Password")]
        public string Password { get; set; }



        [Required(ErrorMessage = "Confirmed Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirmed Password")]
        public string ConfirmedPassword { get; set; }

        [Display(Name = "Mobile Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be a 10 digit number.")]
        public string? PhoneNumber { get; set; }
    }
}
