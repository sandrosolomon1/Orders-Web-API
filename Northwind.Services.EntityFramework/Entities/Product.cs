using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

[Table("Products")]
public class Product
{
    [Key]
    public int ProductID { get; set; }

    public string ProductName { get; set; }

    public int SupplierID { get; set; }

    public int CategoryID { get; set; }

    public string? QuantityPerUnit { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? UnitsInStock { get; set; }

    public int? UnitsOnOrder { get; set; }

    public int? ReorderLevel { get; set; }

    public bool? Discontinued { get; set; }

    // Navigation properties
    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    [ForeignKey("CategoryID")]
    public virtual Category GetCategory { get; set; }

    [ForeignKey("SupplierID")]
    public virtual Supplier GetSupplier { get; set; }

}
