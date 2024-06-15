using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

[Table("Shippers")]
public class Shipper
{
    [Key]
    public int ShipperID { get; set; }

    public string CompanyName { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
}
