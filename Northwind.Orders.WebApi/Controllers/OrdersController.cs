using Microsoft.AspNetCore.Mvc;
using Northwind.Orders.WebApi.Models;
using Northwind.Orders.WebApi.Services;
using Northwind.Services.Repositories;

namespace Northwind.Orders.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderRepository ordersRepository;

    private readonly ILogger<OrdersController> logger;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        this.ordersRepository = orderRepository;
        this.logger = logger;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<FullOrder>> GetOrderAsync(long orderId)
    {
        try
        {
            var order = await this.ordersRepository.GetOrderAsync(orderId);

            return this.Ok(OrdersService.MapToFullOrder(order));
        }
        catch (OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception ex)
        {
#pragma warning disable CA2254 // Template should be a static expression
            this.logger.LogError(ex, $"An error occurred while getting order with id {orderId}.", orderId);
#pragma warning restore CA2254 // Template should be a static expression
            return this.StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BriefOrder>>> GetOrdersAsync([FromQuery] int? skip, [FromQuery] int? count)
    {
        try
        {
            if (skip < 0 || count <= 0 || (skip == 0 && count == 0))
            {
                throw new ArgumentOutOfRangeException(nameof(skip));
            }

            var orders = await this.ordersRepository.GetOrdersAsync(skip ?? 0, count ?? 10);
            return this.Ok(OrdersService.MapToBriefOrders(orders));
        }
        catch (ArgumentOutOfRangeException)
        {
            return this.BadRequest(); // Not working, for some unknown reason, had to throw an error explicitly
        }
        catch (OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting orders.");
            return this.StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AddOrder>> AddOrderAsync(BriefOrder order)
    {
        try
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            long orderId = await this.ordersRepository.AddOrderAsync(OrdersService.MapToRepositoryOrder((long)order?.Id, order));
#pragma warning restore CS8629 // Nullable value type may be null.
            return this.Ok(new AddOrder { OrderId = orderId });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while adding an order.");
            return this.StatusCode(500);
        }
    }

    [HttpDelete("{orderId}")]
    public async Task<ActionResult> RemoveOrderAsync(long orderId)
    {
        try
        {
            await this.ordersRepository.RemoveOrderAsync(orderId);
            return this.NoContent();
        }
        catch (OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, $"An error occurred while removing order");
            return this.StatusCode(500);
        }
    }

    [HttpPut("{orderId}")]
    public async Task<ActionResult> UpdateOrderAsync(long orderId, BriefOrder order)
    {
        try
        {
            await this.ordersRepository.UpdateOrderAsync(OrdersService.MapToRepositoryOrder(orderId, order));
            return this.NoContent();
        }
        catch (OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, $"An error occurred while updating order");
            return this.StatusCode(500);
        }
    }
}
