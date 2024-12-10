using System;
using System.Collections.Generic;

namespace AmlaMarketPlace.DAL.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public int ProductId { get; set; }

    public DateTime OrderTime { get; set; }

    public bool IsApproved { get; set; }

    public virtual User Buyer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User Seller { get; set; } = null!;
}
