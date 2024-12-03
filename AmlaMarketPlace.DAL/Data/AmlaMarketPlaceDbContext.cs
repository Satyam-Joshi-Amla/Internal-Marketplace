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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
