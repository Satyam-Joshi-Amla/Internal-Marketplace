using System.ComponentModel.DataAnnotations;

namespace AmlaMarketPlace.Models.ViewModels.Admin
{
    public class PublishedProductsViewModel
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
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

        [Display(Name = "Is Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Comment")]
        public string CommentForRejecting { get; set; }
    }
}
