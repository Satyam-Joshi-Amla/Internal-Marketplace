using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.ConfigurationManager.CustomValidation
{
    public class CustomPasswordValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            var password = value.ToString();

            if (!password.Any(char.IsLower)) // Check for lowercase
                ErrorMessage = "Password must include at least one lowercase letter.";
            else if (!password.Any(char.IsUpper)) // Check for uppercase
                ErrorMessage = "Password must include at least one uppercase letter.";
            else if (!password.Any(ch => !char.IsLetterOrDigit(ch))) // Check for special character
                ErrorMessage = "Password must include at least one special character.";
            else if (password.Length < 8 || password.Length > 20) // Check length
                ErrorMessage = "Password must be between 8 and 20 characters long.";
            else
                return true;

            return false;
        }
    }

}
