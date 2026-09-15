using KiriyamaServer.Domain.Users;
using KiriyamaServer.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;

public sealed class GenocsContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Credit> Credits { get; set; }
    public DbSet<Debit> Debits { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .ToTable("Account");

        modelBuilder.Entity<Account>()
            .Ignore(p => p.Credits)
            .Ignore(p => p.Debits);

        modelBuilder.Entity<Customer>()
            .ToTable("Customer")
            .Property(b => b.SSN)
            .HasConversion(
                v => v.ToString(),
                v => new SSN(v));

        modelBuilder.Entity<Customer>()
            .ToTable("Customer")
            .Property(b => b.Name)
            .HasConversion(
                v => v.ToString(),
                v => new Name(v));

        modelBuilder.Entity<Customer>()
            .Ignore(p => p.Accounts);

        modelBuilder.Entity<Debit>()
            .ToTable("Debit")
            .Property(b => b.Amount)
            .HasConversion(
                v => v.ToMoney().ToDecimal(),
                v => new PositiveMoney(v));

        modelBuilder.Entity<Credit>()
            .ToTable("Credit")
            .Property(b => b.Amount)
            .HasConversion(
                v => v.ToMoney().ToDecimal(),
                v => new PositiveMoney(v));

        modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                v => v.Value,
                v => new Email(v));

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .OwnsMany(u => u.RefreshTokens, b =>
            {
                b.ToTable("RefreshTokens");
                b.WithOwner().HasForeignKey("UserId");
                b.HasKey("Id");
                b.Property("Id").ValueGeneratedOnAdd();
                b.HasIndex(t => t.TokenHash).IsUnique();
            });

        modelBuilder.Entity<User>()
            .OwnsMany(u => u.AuthorizationCodes, b =>
            {
                b.ToTable("AuthorizationCodes");
                b.WithOwner().HasForeignKey("UserId");
                b.HasKey("Id");
                b.Property("Id").ValueGeneratedOnAdd();
                b.HasIndex(c => c.Code).IsUnique();
            });
    }
}