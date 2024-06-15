using Northwind.Services.Repositories;
using EntityOrder = Northwind.Services.EntityFramework.Entities.Order;

namespace Northwind.Services.EntityFramework.Repositories;
internal class OrderRepositoryHelper
{
    public static Order MapToRepositoryOrder(EntityOrder order)
    {
        Order rOrder = new Order(order.OrderID)
        {
            Customer = new Customer(new CustomerCode(order.CustomerID))
            {
                CompanyName = order.GetCustomer.CompanyName,
            },
            Employee = new Employee(order.EmployeeID)
            {
                FirstName = order.GetEmployee.FirstName,
                LastName = order.GetEmployee.LastName,
                Country = order.GetEmployee.Country,
            },
            Shipper = new Shipper(order.ShipVia)
            {
                CompanyName = order.GetShipper.CompanyName,
            },
            ShippingAddress = new ShippingAddress(order.ShipAddress, order.ShipCity, order.ShipRegion, order.ShipPostalCode, order.ShipCountry),
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Freight = (double)order.Freight,
            ShipName = order.ShipName,
        };

        foreach (Entities.OrderDetail orderDetail in order.GetOrderDetails)
        {
            var repositoryOrderDetail = new OrderDetail(rOrder)
            {
                Product = new Product(orderDetail.Product.ProductID)
                {
                    CategoryId = orderDetail.Product.CategoryID,
                    Category = orderDetail.Product.GetCategory.CategoryName,
                    SupplierId = orderDetail.Product.SupplierID,
                    Supplier = orderDetail.Product.GetSupplier.CompanyName,
                    ProductName = orderDetail.Product.ProductName,
                },
                UnitPrice = (double)orderDetail.UnitPrice,
                Quantity = orderDetail.Quantity,
                Discount = (float)orderDetail.Discount,
            };
            rOrder.OrderDetails.Add(repositoryOrderDetail);
        }

        return rOrder;
    }

    public static bool ValidateOrder(Order order)
    {
        if (order is null || order.Id <= 0 || order.Customer is null || order.Employee is null)
        {
            return false;
        }

        return order.OrderDetails.All(orderDetail =>
            orderDetail.Order != null && orderDetail.Order.Id > 0 &&
            orderDetail.Product != null && orderDetail.Product.Id > 0 &&
            orderDetail.UnitPrice > 0 &&
            orderDetail.Quantity > 0 &&
            orderDetail.Discount >= 0);
    }
}
