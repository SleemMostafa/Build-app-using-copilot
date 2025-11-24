using CoffeeRestaurant.Application.Common.Interfaces;
using CoffeeRestaurant.Domain.Entities;
using MediatR;

namespace CoffeeRestaurant.Application.CoffeeItems.Commands.CreateCoffeeItem;

public record CreateCoffeeItemCommand : IRequest<CreateCoffeeItemResponse>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public string? ImageUrl { get; init; }
}

public class CreateCoffeeItemCommandHandler : IRequestHandler<CreateCoffeeItemCommand, CreateCoffeeItemResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateCoffeeItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateCoffeeItemResponse> Handle(CreateCoffeeItemCommand request, CancellationToken cancellationToken)
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

        return new CreateCoffeeItemResponse
        {
            Id = coffeeItem.Id,
            Name = coffeeItem.Name,
            Description = coffeeItem.Description,
            Price = coffeeItem.Price,
            IsAvailable = coffeeItem.IsAvailable,
            ImageUrl = coffeeItem.ImageUrl,
            CategoryId = coffeeItem.CategoryId,
            CreatedAt = coffeeItem.CreatedAt,
            UpdatedAt = coffeeItem.UpdatedAt
        };
    }
}

public record CreateCoffeeItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public string? ImageUrl { get; init; }
    public Guid CategoryId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
