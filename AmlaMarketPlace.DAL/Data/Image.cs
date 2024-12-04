using System;
using System.Collections.Generic;

namespace AmlaMarketPlace.DAL.Data;

public partial class Image
{
    public int ImageId { get; set; }

    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Link { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
