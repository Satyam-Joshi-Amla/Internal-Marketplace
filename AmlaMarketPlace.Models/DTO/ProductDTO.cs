using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public int Inventory { get; set; }

        public int StatusId { get; set; }

        [Display(Name = "Status")]
        public string StatusValue { get; set; }

        [Display(Name = "Is Published ?")]
        public bool IsPublished { get; set; }
        [Display(Name = "Comment")]
        public string CommentForRejecting { get; set; }
    }
}
