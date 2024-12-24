namespace AmlaMarketPlace.DAL.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public int ProductId { get; set; }

    public DateTime OrderTime { get; set; }

    public int IsApproved { get; set; }

    public int Quantity { get; set; }

    public string RejectComment { get; set; } = null!;

    public virtual User Buyer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User Seller { get; set; } = null!;
}
