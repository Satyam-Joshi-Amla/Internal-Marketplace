using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Email")]
        public string EmailAddress { get; set; } = null!;

        [Display(Name = "Is Email Verified ?")]
        public bool IsEmailVerified { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Display(Name = "Mobile Number")]
        public string? MobileNumber { get; set; }

        [Display(Name = "Is Mobile Number Verified ?")]
        public bool? IsmobileNumberVerified { get; set; }

        public int UserRoleId { get; set; }

        [Display(Name = "User Role")]
        public string UserRole { get; set; }

        [Display(Name = "Sign up on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last modified time")]
        public DateTime EditedOn { get; set; }

        public string? VerificationToken { get; set; }

        public DateTime? TokenExpiration { get; set; }
    }
}
