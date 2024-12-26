namespace AmlaMarketPlace.DAL.Data;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
