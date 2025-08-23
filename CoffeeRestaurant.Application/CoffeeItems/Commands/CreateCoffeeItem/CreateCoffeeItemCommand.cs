using CoffeeRestaurant.Application.Common.Interfaces;
using CoffeeRestaurant.Domain.Entities;
using CoffeeRestaurant.Shared.DTOs;
using CoffeeRestaurant.Application.Mappers;
using MediatR;

namespace CoffeeRestaurant.Application.CoffeeItems.Commands.CreateCoffeeItem;

public record CreateCoffeeItemCommand : IRequest<CoffeeItemDto>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public string? ImageUrl { get; init; }
}

public class CreateCoffeeItemCommandHandler : IRequestHandler<CreateCoffeeItemCommand, CoffeeItemDto>
{
    private readonly IApplicationDbContext _context;

    public CreateCoffeeItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CoffeeItemDto> Handle(CreateCoffeeItemCommand request, CancellationToken cancellationToken)
    {
        var coffeeItem = new CoffeeItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        _context.CoffeeItems.Add(coffeeItem);
        await _context.SaveChangesAsync(cancellationToken);

        // Use manual mapping instead of AutoMapper
        return coffeeItem.ToDto();
    }
}
