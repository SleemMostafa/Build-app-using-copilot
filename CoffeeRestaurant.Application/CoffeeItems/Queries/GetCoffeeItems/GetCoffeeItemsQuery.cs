using CoffeeRestaurant.Application.Common.Interfaces;
using CoffeeRestaurant.Shared.DTOs;
using CoffeeRestaurant.Domain.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoffeeRestaurant.Application.CoffeeItems.Queries.GetCoffeeItems;

public record GetCoffeeItemsQuery : IRequest<List<CoffeeItemDto>>;

public class GetCoffeeItemsQueryHandler : IRequestHandler<GetCoffeeItemsQuery, List<CoffeeItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCoffeeItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CoffeeItemDto>> Handle(GetCoffeeItemsQuery request, CancellationToken cancellationToken)
    {
        var coffeeItems = await _context.CoffeeItems
            .Include(ci => ci.Category)
            .Where(ci => ci.IsAvailable)
            .ToListAsync(cancellationToken);

        // Use manual mapping instead of AutoMapper
        return coffeeItems.ToDtoList();
    }
}
