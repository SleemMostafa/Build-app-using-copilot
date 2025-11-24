using CoffeeRestaurant.Application.Common.Interfaces;
using CoffeeRestaurant.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoffeeRestaurant.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<CreateOrderResponse>
{
    public Guid CustomerId { get; init; }
    public List<CreateOrderItemRequest> OrderItems { get; init; } = new();
    public string? Notes { get; init; }
}

public record CreateOrderItemRequest
{
    public Guid CoffeeItemId { get; init; }
    public int Quantity { get; init; }
    public string? SpecialInstructions { get; init; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Get coffee items to calculate prices
        var coffeeItemIds = request.OrderItems.Select(oi => oi.CoffeeItemId).ToList();
        var coffeeItems = await _context.CoffeeItems
            .Where(ci => coffeeItemIds.Contains(ci.Id))
            .ToDictionaryAsync(ci => ci.Id, cancellationToken);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        var orderItems = new List<OrderItem>();
        decimal totalPrice = 0;

        foreach (var itemRequest in request.OrderItems)
        {
            if (coffeeItems.TryGetValue(itemRequest.CoffeeItemId, out var coffeeItem))
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    CoffeeItemId = itemRequest.CoffeeItemId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = coffeeItem.Price,
                    Subtotal = coffeeItem.Price * itemRequest.Quantity,
                    SpecialInstructions = itemRequest.SpecialInstructions,
                    CreatedAt = DateTime.UtcNow
                };

                orderItems.Add(orderItem);
                totalPrice += orderItem.Subtotal;
            }
        }

        order.TotalPrice = totalPrice;
        order.OrderItems = orderItems;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateOrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            OrderItems = orderItems.Select(oi => new CreateOrderItemResponse
            {
                Id = oi.Id,
                CoffeeItemId = oi.CoffeeItemId,
                CoffeeItemName = coffeeItems[oi.CoffeeItemId].Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Subtotal = oi.Subtotal,
                SpecialInstructions = oi.SpecialInstructions
            }).ToList()
        };
    }
}

public record CreateOrderResponse
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalPrice { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<CreateOrderItemResponse> OrderItems { get; init; } = new();
}

public record CreateOrderItemResponse
{
    public Guid Id { get; init; }
    public Guid CoffeeItemId { get; init; }
    public string CoffeeItemName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
    public string? SpecialInstructions { get; init; }
}
