using System;
using System.Collections.Generic;

namespace AmlaMarketPlace.DAL.Data;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public string Password { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public bool? IsmobileNumberVerified { get; set; }

    public int UserRoleId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime EditedOn { get; set; }

    public string? VerificationToken { get; set; }

    public DateTime? TokenExpiration { get; set; }

    public virtual ICollection<Order> OrderBuyers { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderSellers { get; set; } = new List<Order>();

    public virtual UserRole UserRole { get; set; } = null!;
}
