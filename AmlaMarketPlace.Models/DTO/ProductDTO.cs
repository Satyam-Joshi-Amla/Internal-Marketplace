using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public int Inventory { get; set; }

        public int StatusId { get; set; }

        public bool IsPublished { get; set; }
    }
}
