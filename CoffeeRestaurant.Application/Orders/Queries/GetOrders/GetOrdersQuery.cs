using CoffeeRestaurant.Application.Common.Interfaces;
using CoffeeRestaurant.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoffeeRestaurant.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<List<OrderDto>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Barista)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.CoffeeItem)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            BaristaId = o.BaristaId,
            OrderDate = o.OrderDate,
            TotalPrice = o.TotalPrice,
            Status = OrderStatus.Pending,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Customer = new CustomerDto
            {
                Id = o.Customer.Id,
                Name = o.Customer.Name,
                Email = o.Customer.Email,
                Phone = o.Customer.Phone,
                Address = o.Customer.Address,
                CreatedAt = o.Customer.CreatedAt,
                UpdatedAt = o.Customer.UpdatedAt
            },
            Barista = o.Barista != null ? new BaristaDto
            {
                Id = o.Barista.Id,
                UserId = o.Barista.UserId,
                Name = o.Barista.Name,
                IsActive = o.Barista.IsActive,
                CreatedAt = o.Barista.CreatedAt,
                UpdatedAt = o.Barista.UpdatedAt
            } : null,
            OrderItems = o.OrderItems.Select(oi => new OrderItemDto
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
                CoffeeItem = new CoffeeItemDto
                {
                    Id = oi.CoffeeItem.Id,
                    Name = oi.CoffeeItem.Name,
                    Description = oi.CoffeeItem.Description,
                    Price = oi.CoffeeItem.Price,
                    IsAvailable = oi.CoffeeItem.IsAvailable,
                    ImageUrl = oi.CoffeeItem.ImageUrl,
                    CategoryId = oi.CoffeeItem.CategoryId,
                    CreatedAt = oi.CoffeeItem.CreatedAt,
                    UpdatedAt = oi.CoffeeItem.UpdatedAt,
                    Category = new CategoryDto() // This would be loaded from DB
                }
            }).ToList()
        }).ToList();
    }
}
