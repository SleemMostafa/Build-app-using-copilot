using CoffeeRestaurant.Application.Common.Interfaces;
using CoffeeRestaurant.Domain.Entities;
using CoffeeRestaurant.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderStatus = CoffeeRestaurant.Domain.Entities.OrderStatus;

namespace CoffeeRestaurant.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<OrderDto>
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

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _context;

    public CreateOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
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

        // In a real application, you would use AutoMapper here
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            BaristaId = order.BaristaId,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Status = Shared.DTOs.OrderStatus.Pending,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Customer = new CustomerDto(), // This would be loaded from DB
            Barista = null,
            OrderItems = orderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                OrderId = oi.OrderId,
                CoffeeItemId = oi.CoffeeItemId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Subtotal = oi.Subtotal,
                SpecialInstructions = oi.SpecialInstructions,
                CreatedAt = oi.CreatedAt,
                UpdatedAt = oi.UpdatedAt,
                CoffeeItem = new CoffeeItemDto() // This would be loaded from DB
            }).ToList()
        };
    }
}
