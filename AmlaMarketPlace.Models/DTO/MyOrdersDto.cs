using System.ComponentModel.DataAnnotations;

namespace AmlaMarketPlace.Models.DTO
{
    public class MyOrdersDto
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
        [Display(Name = "Status")]

        public int IsApproved { get; set; }
        public int Quantity { get; set; }
        public string RejectComment { get; set; }
    }
}
