namespace AmlaMarketPlace.DAL.Data;

public partial class Comment
{
    public int CommentId { get; set; }

    public int ProductId { get; set; }

    public string Comment1 { get; set; } = null!;

    public DateTime Date { get; set; }
}
