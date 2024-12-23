using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AmlaMarketPlace.DAL.Data;

public partial class AmlaMarketPlaceDbContext : DbContext
{
    public AmlaMarketPlaceDbContext()
    {
    }

    public AmlaMarketPlaceDbContext(DbContextOptions<AmlaMarketPlaceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductComment> ProductComments { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>(entity =>
        {
            entity.Property(e => e.Link).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Images)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Images_Products");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderTime).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.RejectComment)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("none");

            entity.HasOne(d => d.Buyer).WithMany(p => p.OrderBuyers)
                .HasForeignKey(d => d.BuyerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Products");

            entity.HasOne(d => d.Seller).WithMany(p => p.OrderSellers)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.IsPublished).HasDefaultValue(true);
            entity.Property(e => e.ModifiedOn).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.StatusId).HasDefaultValue(1);
        });

        modelBuilder.Entity<ProductComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_Comments");

            entity.Property(e => e.Date).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.RejectedComments).IsUnicode(false);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
            entity.Property(e => e.StatusValue)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EditedOn).HasColumnType("datetime");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TokenExpiration).HasColumnType("datetime");
            entity.Property(e => e.VerificationToken).HasMaxLength(255);

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Roles");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Roles");

            entity.ToTable("UserRole");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
