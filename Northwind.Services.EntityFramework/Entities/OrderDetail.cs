using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Services.EntityFramework.Entities;

[Table("OrderDetails")]
public class OrderDetail
{
    public int OrderID { get; set; }

    public int ProductID { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public float Discount { get; set; }

    [ForeignKey("OrderID")]
    public virtual Order Order { get; set; }

    [ForeignKey("ProductID")]
    public virtual Product Product { get; set; }
}
