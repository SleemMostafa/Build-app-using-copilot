using CoffeeRestaurant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoffeeRestaurant.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<List<GetOrdersResponse>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<GetOrdersResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetOrdersResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Barista)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.CoffeeItem)
                    .ThenInclude(ci => ci.Category)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new GetOrdersResponse
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                CustomerEmail = o.Customer.Email,
                CustomerPhone = o.Customer.Phone,
                BaristaId = o.BaristaId,
                BaristaName = o.Barista != null ? o.Barista.Name : null,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                OrderItems = o.OrderItems.Select(oi => new GetOrderItemResponse
                {
                    Id = oi.Id,
                    CoffeeItemId = oi.CoffeeItemId,
                    CoffeeItemName = oi.CoffeeItem.Name,
                    CoffeeItemDescription = oi.CoffeeItem.Description,
                    CategoryName = oi.CoffeeItem.Category.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal,
                    SpecialInstructions = oi.SpecialInstructions
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return orders;
    }
}

public record GetOrdersResponse
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string? CustomerPhone { get; init; }
    public Guid? BaristaId { get; init; }
    public string? BaristaName { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalPrice { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<GetOrderItemResponse> OrderItems { get; init; } = new();
}

public record GetOrderItemResponse
{
    public Guid Id { get; init; }
    public Guid CoffeeItemId { get; init; }
    public string CoffeeItemName { get; init; } = string.Empty;
    public string CoffeeItemDescription { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
    public string? SpecialInstructions { get; init; }
}
