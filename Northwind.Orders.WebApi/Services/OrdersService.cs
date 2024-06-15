using Northwind.Orders.WebApi.Models;
using Northwind.Services.Repositories;

namespace Northwind.Orders.WebApi.Services;

public static class OrdersService
{
    public static Order MapToRepositoryOrder(long orderId, BriefOrder order)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        Order rOrder = new Order(orderId)
        {
            Customer = new Northwind.Services.Repositories.Customer(new CustomerCode(order.CustomerId)),
            Employee = new Northwind.Services.Repositories.Employee(order.EmployeeId),
            Shipper = new Northwind.Services.Repositories.Shipper(order.ShipperId),
            ShippingAddress = new Northwind.Services.Repositories.ShippingAddress(order.ShipAddress, order.ShipCity, order.ShipRegion, order.ShipPostalCode, order.ShipCountry),
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Freight = order.Freight,
            ShipName = order.ShipName,
        };
#pragma warning restore CA1062 // Validate arguments of public methods

        foreach (BriefOrderDetail orderDetail in order.OrderDetails)
        {
            var repositoryOrderDetail = new OrderDetail(rOrder)
            {
                Product = new Product(orderDetail.ProductId),
                UnitPrice = orderDetail.UnitPrice,
                Quantity = orderDetail.Quantity,
                Discount = orderDetail.Discount,
            };
            rOrder.OrderDetails.Add(repositoryOrderDetail);
        }

        return rOrder;
    }

    public static IEnumerable<BriefOrder> MapToBriefOrders(IList<Order> orders)
    {
        var briefOrders = new List<BriefOrder>();

        foreach (var order in orders)
        {
            var orderDetails = new List<BriefOrderDetail>();

            foreach (var o in order.OrderDetails)
            {
                orderDetails.Add(new BriefOrderDetail
                {
                    ProductId = o.Product.Id,
                    UnitPrice = o.UnitPrice,
                    Quantity = o.Quantity,
                    Discount = o.Discount,
                });
            }

            briefOrders.Add(new BriefOrder
            {
                Id = order.Id,
                CustomerId = order.Customer.Code.Code,
                EmployeeId = order.Employee.Id,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                ShipperId = order.Shipper.Id,
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShipAddress = order.ShippingAddress.Address,
                ShipCity = order.ShippingAddress.City,
                ShipRegion = order.ShippingAddress.Region,
                ShipPostalCode = order.ShippingAddress.PostalCode,
                ShipCountry = order.ShippingAddress.Country,
                OrderDetails = orderDetails,
            });
        }

        return briefOrders;
    }

    public static FullOrder MapToFullOrder(Order order)
    {
        var fullOrderDetails = new List<FullOrderDetail>();

        foreach (var o in order.OrderDetails)
        {
            fullOrderDetails.Add(new FullOrderDetail
            {
                ProductId = o.Product.Id,
                ProductName = o.Product.ProductName,
                CategoryId = o.Product.CategoryId,
                CategoryName = o.Product.Category,
                SupplierId = o.Product.SupplierId,
                SupplierCompanyName = o.Product.Supplier,
                UnitPrice = o.UnitPrice,
                Quantity = o.Quantity,
                Discount = o.Discount,
            });
        }

        return new FullOrder
        {
            Id = order.Id,
            Customer = new Models.Customer
            {
                Code = order.Customer.Code.Code,
                CompanyName = order.Customer.CompanyName,
            },
            Employee = new Models.Employee
            {
                Id = order.Employee.Id,
                FirstName = order.Employee.FirstName,
                LastName = order.Employee.LastName,
                Country = order.Employee.Country,
            },
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = new Models.Shipper
            {
                Id = order.Shipper.Id,
                CompanyName = order.Shipper.CompanyName,
            },
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShippingAddress = new Models.ShippingAddress
            {
                Address = order.ShippingAddress.Address,
                City = order.ShippingAddress.City,
                Region = order.ShippingAddress.Region,
                PostalCode = order.ShippingAddress.PostalCode,
                Country = order.ShippingAddress.Country,
            },
            OrderDetails = fullOrderDetails,
        };
    }
}
