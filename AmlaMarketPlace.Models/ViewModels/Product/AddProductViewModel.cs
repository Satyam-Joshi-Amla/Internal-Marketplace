using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmlaMarketPlace.Models.ViewModels.Validations;

using Microsoft.AspNetCore.Http;

namespace AmlaMarketPlace.Models.ViewModels.Product
{
    public class AddProductViewModel
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 500 characters.")]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 9999999, ErrorMessage = "Price must between 0.01 and 99,99,999.")]
        public decimal Price { get; set; }

        [StringLength(50000, ErrorMessage = "Description can't be longer than 50000 characters.")]
        public string Description { get; set; }

        [Required]
        [Range(1, 99999, ErrorMessage = "Inventory must be between 1 and 99999.")]
        public int Inventory { get; set; }

        [Required]
        [ImageValidation(ErrorMessage = "Only image files (JPG, PNG) are allowed.")]
        public IFormFile Image { get; set; }
    }
}
