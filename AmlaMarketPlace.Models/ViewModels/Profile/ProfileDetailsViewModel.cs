using AmlaMarketPlace.Models.ViewModels.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.ViewModels.Profile
{
    public class ProfileDetailsViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

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
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be a 10 digit number.")]
        public string MobileNumber { get; set; }
    }
}
