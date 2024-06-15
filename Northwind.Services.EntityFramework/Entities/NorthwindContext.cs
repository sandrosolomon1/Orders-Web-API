using Microsoft.EntityFrameworkCore;

namespace Northwind.Services.EntityFramework.Entities;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<Shipper> Shippers { get; set; }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<OrderDetail>()
            .HasKey(bc => new { bc.OrderID, bc.ProductID });

        _ = modelBuilder.Entity<OrderDetail>()
            .HasOne(bc => bc.Product)
            .WithMany(c => c.OrderDetails)
            .HasForeignKey(bc => bc.ProductID)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<OrderDetail>()
            .HasOne(bc => bc.Order)
            .WithMany(c => c.GetOrderDetails)
            .HasForeignKey(bc => bc.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Order>()
            .HasOne(bc => bc.GetEmployee)
            .WithMany(b => b.Orders)
            .HasForeignKey(bc => bc.EmployeeID)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Order>()
            .HasOne(bc => bc.GetCustomer)
            .WithMany(b => b.Orders)
            .HasForeignKey(bc => bc.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Order>()
            .HasOne(bc => bc.GetShipper)
            .WithMany(b => b.Orders)
            .HasForeignKey(bc => bc.ShipVia)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
