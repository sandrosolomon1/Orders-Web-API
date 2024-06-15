using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using RepositoryOrder = Northwind.Services.Repositories.Order;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext context;

    public OrderRepository(NorthwindContext context)
    {
        this.context = context;
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        try
        {
            Entities.Order? order = await this.context.Orders.Include(o => o.GetCustomer)
                                            .Include(o => o.GetEmployee)
                                            .Include(o => o.GetShipper)
                                            .Include(o => o.GetOrderDetails)
                                                .ThenInclude(od => od.Product)
                                                    .ThenInclude(p => p.GetCategory)
                                            .Include(o => o.GetOrderDetails)
                                                .ThenInclude(od => od.Product)
                                                    .ThenInclude(p => p.GetSupplier)
                                            .SingleOrDefaultAsync(o => o.OrderID == orderId)
                                            ?? throw new OrderNotFoundException();

            RepositoryOrder repoOrder = OrderRepositoryHelper.MapToRepositoryOrder(order);
            return repoOrder;
        }
        catch (OrderNotFoundException)
        {
            throw;
        }
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        try
        {
            if (skip < 0 || count <= 0 || (skip == 0 && count == 0))
            {
                throw new ArgumentOutOfRangeException(nameof(skip));
            }

            List<RepositoryOrder> repoOrders = new List<RepositoryOrder>();

            List<Entities.Order> orders = await this.context.Orders.Skip(skip).Take(count)
                                                .Include(o => o.GetCustomer)
                                                .Include(o => o.GetEmployee)
                                                .Include(o => o.GetShipper)
                                                .Include(o => o.GetOrderDetails)
                                                    .ThenInclude(od => od.Product)
                                                        .ThenInclude(p => p.GetCategory)
                                                .Include(o => o.GetOrderDetails)
                                                    .ThenInclude(od => od.Product)
                                                        .ThenInclude(p => p.GetSupplier)
                                                .OrderBy(o => o.OrderID)
                                                .ToListAsync();

            foreach (Entities.Order o in orders)
            {
                repoOrders.Add(OrderRepositoryHelper.MapToRepositoryOrder(o));
            }

            return repoOrders;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw;
        }
    }

    public async Task<long> AddOrderAsync(RepositoryOrder order)
    {
        try
        {
            if (!OrderRepositoryHelper.ValidateOrder(order))
            {
                throw new RepositoryException();
            }

            Entities.Order? isOrderExist = await this.context.Orders.SingleOrDefaultAsync(o => o.OrderID == order.Id);

            if (isOrderExist is not null)
            {
#pragma warning disable CA1062 // Validate arguments of public methods
                return order.Id;
#pragma warning restore CA1062 // Validate arguments of public methods
            }

            Entities.Order entityOrder = new Entities.Order
            {
                OrderID = (int)order.Id,
                CustomerID = order.Customer.Code.Code,
                EmployeeID = (int)order.Employee.Id,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                ShipVia = (int)order.Shipper.Id,
                Freight = (decimal)order.Freight,
                ShipName = order.ShipName,
                ShipAddress = order.ShippingAddress.Address,
                ShipCity = order.ShippingAddress.City,
                ShipRegion = order.ShippingAddress.Region,
                ShipPostalCode = order.ShippingAddress.PostalCode,
                ShipCountry = order.ShippingAddress.Country,
            };

            _ = this.context.Orders.Add(entityOrder);

            foreach (Services.Repositories.OrderDetail od in order.OrderDetails)
            {
                Entities.OrderDetail entityOd = new Entities.OrderDetail
                {
                    OrderID = (int)od.Order.Id,
                    ProductID = (int)od.Product.Id,
                    UnitPrice = (decimal)od.UnitPrice,
                    Quantity = (int)od.Quantity,
                    Discount = (float)od.Discount,
                };

                _ = this.context.OrderDetails.Add(entityOd);
            }

            _ = await this.context.SaveChangesAsync();
            return order.Id;
        }
        catch (RepositoryException)
        {
            throw;
        }
    }

    public async Task RemoveOrderAsync(long orderId)
    {
        try
        {
            Entities.Order? order = await this.context.Orders.Include(o => o.GetOrderDetails).SingleOrDefaultAsync(o => o.OrderID == orderId)
                                    ?? throw new OrderNotFoundException();

            _ = this.context.Orders.Remove(order);

            this.context.OrderDetails.RemoveRange(order.GetOrderDetails.ToList());

            _ = await this.context.SaveChangesAsync();
        }
        catch (OrderNotFoundException)
        {
            throw;
        }
    }

    public async Task UpdateOrderAsync(RepositoryOrder order)
    {
        try
        {
            Entities.Order? entityOrder = await this.context.Orders.Include(o => o.GetCustomer)
                                            .Include(o => o.GetEmployee)
                                            .Include(o => o.GetShipper)
                                            .Include(o => o.GetOrderDetails)
                                                .ThenInclude(od => od.Product)
                                                    .ThenInclude(p => p.GetCategory)
                                            .Include(o => o.GetOrderDetails)
                                                .ThenInclude(od => od.Product)
                                                    .ThenInclude(p => p.GetSupplier)
                                            .SingleOrDefaultAsync(o => o.OrderID == order.Id)
                                            ?? throw new OrderNotFoundException();

#pragma warning disable CA1062 // Validate arguments of public methods
            entityOrder.OrderID = (int)order.Id;
#pragma warning restore CA1062 // Validate arguments of public methods
            entityOrder.CustomerID = order.Customer.Code.Code;
            entityOrder.EmployeeID = (int)order.Employee.Id;
            entityOrder.OrderDate = order.OrderDate;
            entityOrder.RequiredDate = order.RequiredDate;
            entityOrder.ShippedDate = order.ShippedDate;
            entityOrder.ShipVia = (int)order.Shipper.Id;
            entityOrder.Freight = (decimal)order.Freight;
            entityOrder.ShipName = order.ShipName;
            entityOrder.ShipAddress = order.ShippingAddress.Address;
            entityOrder.ShipCity = order.ShippingAddress.City;
            entityOrder.ShipRegion = order.ShippingAddress.Region;
            entityOrder.ShipPostalCode = order.ShippingAddress.PostalCode;
            entityOrder.ShipCountry = order.ShippingAddress.Country;

            _ = this.context.Orders.Update(entityOrder);

            this.context.OrderDetails.RemoveRange(entityOrder.GetOrderDetails.ToList());

            foreach (Services.Repositories.OrderDetail sod in order.OrderDetails)
            {
                Entities.OrderDetail newod = new Entities.OrderDetail
                {
                    OrderID = (int)sod.Order.Id,
                    ProductID = (int)sod.Product.Id,
                    UnitPrice = (decimal)sod.UnitPrice,
                    Quantity = (int)sod.Quantity,
                    Discount = (float)sod.Discount,
                };
                _ = this.context.OrderDetails.Add(newod);
            }

            _ = await this.context.SaveChangesAsync();
        }
        catch (OrderNotFoundException)
        {
            throw;
        }
    }
}
