using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace AmlaMarketPlace.Models.ViewModels.Validations
{
    public class ImageValidation : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ImageValidation() : base("The file must be an image (JPG, PNG).")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file == null)
            {
                return ValidationResult.Success;
            }

            // Get the file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            // Check if the extension is valid
            if (Array.Exists(_allowedExtensions, ext => ext == fileExtension))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage ?? "Invalid file type. Only image files (JPG, PNG) are allowed.");
            }
        }
    }
}
