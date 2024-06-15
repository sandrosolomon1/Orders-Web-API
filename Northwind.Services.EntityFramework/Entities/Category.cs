using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Services.EntityFramework.Entities;

[Table("Categories")]
public class Category
{
    [Key]
    public int CategoryID { get; set; }

    public string CategoryName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}
