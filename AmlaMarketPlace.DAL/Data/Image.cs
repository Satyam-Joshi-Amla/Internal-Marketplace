namespace AmlaMarketPlace.DAL.Data;

public partial class Image
{
    public int ImageId { get; set; }

    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Link { get; set; } = null!;

    public bool? IsDefault { get; set; }

    public virtual Product Product { get; set; } = null!;
}
