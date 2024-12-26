namespace AmlaMarketPlace.DAL.Data;

public partial class ProductComment
{
    public int CommentId { get; set; }

    public int ProductId { get; set; }

    public string RejectedComments { get; set; } = null!;

    public DateTime Date { get; set; }
}
