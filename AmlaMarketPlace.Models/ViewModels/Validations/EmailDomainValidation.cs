using System.ComponentModel.DataAnnotations;

namespace AmlaMarketPlace.Models.ViewModels.Validations
{
    public class EmailDomainValidation : ValidationAttribute
    {
        private readonly string _domain;

        public EmailDomainValidation(string domain)
        {
            _domain = domain;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Allow null or empty for now
            }

            var email = value.ToString();
            var domain = _domain;

            // Check if the email ends with the specified domain
            if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success; // Valid email
            }
            else
            {
                return new ValidationResult($"Email must end with '{domain}'");
            }
        }
    }
}
