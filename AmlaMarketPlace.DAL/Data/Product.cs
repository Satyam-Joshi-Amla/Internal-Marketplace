using System;
using System.Collections.Generic;

namespace AmlaMarketPlace.DAL.Data;

public partial class Product
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

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
