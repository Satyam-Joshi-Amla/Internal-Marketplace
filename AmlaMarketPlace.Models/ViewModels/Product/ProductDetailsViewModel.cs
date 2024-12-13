using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int Inventory { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public int StatusId { get; set; }
        public bool IsPublished { get; set; }
        //public string ImageLink { get; set; }
        public List<ImageViewModel> Images { get; set; }
    }

    public class ImageViewModel
    {
        public string ImagePath { get; set; }
        public bool IsDefault { get; set; }
    }

}
