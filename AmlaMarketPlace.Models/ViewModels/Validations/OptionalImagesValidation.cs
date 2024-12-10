using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace AmlaMarketPlace.Models.ViewModels.Validations
{
    public class OptionalImagesValidation : ValidationAttribute
    {
        private readonly int _maxFiles;
        private readonly List<string> _allowedExtensions;

        public OptionalImagesValidation(int maxFiles, string[] allowedExtensions)
        {
            _maxFiles = maxFiles;
            _allowedExtensions = allowedExtensions.Select(e => e.ToLower()).ToList();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;
            if (files == null || !files.Any())
            {
                // Optional, so no files is valid
                return ValidationResult.Success;
            }

            if (files.Count > _maxFiles)
            {
                return new ValidationResult($"You can upload a maximum of {_maxFiles} files.");
            }

            foreach (var file in files)
            {
                var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult($"Only the following file types are allowed: {string.Join(", ", _allowedExtensions)}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

