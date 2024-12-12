using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int BuyerId { get; set; }
        [Display(Name = "Buyer Name")]
        public string BuyerName { get; set; }
        public int SellerId { get; set; }
        [Display(Name = "Seller Name")]
        public string SellerName { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Order Time")]
        public DateTime OrderTime { get; set; }
        public bool IsApproved { get; set; }
    }
}
