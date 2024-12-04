using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace AmlaMarketPlace.Models.ViewModels.Product
{
    public class AddProductViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Inventory { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
