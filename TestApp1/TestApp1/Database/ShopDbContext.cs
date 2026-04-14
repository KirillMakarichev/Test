using Microsoft.EntityFrameworkCore;
using TestApp1.Database.Models;

namespace TestApp1.Database;

public class ShopDbContext : DbContext
{
    private static readonly Guid CustomerRoleId = Guid.Parse("8C2A75D7-D4A4-4DF3-8B4D-906D41E71A6F");
    private static readonly Guid ManagerRoleId = Guid.Parse("D4683852-3B8E-4E74-BD15-442981CA4AE0");
    private static readonly Guid AdminCustomerId = Guid.Parse("6DE58BAF-ECF8-4D72-BAE9-A67C8D1F8E6B");

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasOne(x => x.Role)
            .WithMany(x => x.Customers)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(x => x.Name);

        modelBuilder.Entity<Order>()
            .HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(x => x.ItemId)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasIndex(x => x.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasIndex(x => new { x.Status, x.OrderDate });

        modelBuilder.Entity<Order>()
            .HasIndex(x => new { x.CustomerId, x.OrderDate });

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = CustomerRoleId,
                Name = UserRole.Customer
            },
            new Role
            {
                Id = ManagerRoleId,
                Name = UserRole.Manager
            });

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = AdminCustomerId,
                Name = "admin",
                PasswordHash = "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918",
                Code = "ADM-0001",
                Address = "System",
                Discount = 0,
                RoleId = ManagerRoleId
            });
    }
}