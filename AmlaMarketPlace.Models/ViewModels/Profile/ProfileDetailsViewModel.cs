using AmlaMarketPlace.Models.ViewModels.Validations;
using System.ComponentModel.DataAnnotations;

namespace AmlaMarketPlace.Models.ViewModels.Profile
{
    public class ProfileDetailsViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email.")]
        [Display(Name = "Email Address")]
        [EmailDomainValidation("amla.io")]
        public string EmailAddress { get; set; }

        [Display(Name = "Is Email Verified")]
        public bool IsEmailVerified { get; set; }

        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\d{10})?$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string? MobileNumber { get; set; }
    }
}
