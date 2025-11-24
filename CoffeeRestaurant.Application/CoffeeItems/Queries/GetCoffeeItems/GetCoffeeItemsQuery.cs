using CoffeeRestaurant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoffeeRestaurant.Application.CoffeeItems.Queries.GetCoffeeItems;

public record GetCoffeeItemsQuery : IRequest<List<GetCoffeeItemsResponse>>;

public class GetCoffeeItemsQueryHandler : IRequestHandler<GetCoffeeItemsQuery, List<GetCoffeeItemsResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCoffeeItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetCoffeeItemsResponse>> Handle(GetCoffeeItemsQuery request, CancellationToken cancellationToken)
    {
        var coffeeItems = await _context.CoffeeItems
            .Include(ci => ci.Category)
            .Where(ci => ci.IsAvailable)
            .Select(ci => new GetCoffeeItemsResponse
            {
                Id = ci.Id,
                Name = ci.Name,
                Description = ci.Description,
                Price = ci.Price,
                IsAvailable = ci.IsAvailable,
                ImageUrl = ci.ImageUrl,
                CategoryId = ci.CategoryId,
                CategoryName = ci.Category.Name,
                CategoryDescription = ci.Category.Description,
                CreatedAt = ci.CreatedAt,
                UpdatedAt = ci.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return coffeeItems;
    }
}

public record GetCoffeeItemsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public string? ImageUrl { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string? CategoryDescription { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
