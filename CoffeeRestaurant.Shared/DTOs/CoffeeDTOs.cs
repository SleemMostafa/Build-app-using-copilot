namespace CoffeeRestaurant.Shared.DTOs;

public class CategoryDto : BaseDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public class CoffeeItemDto : BaseDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public string? ImageUrl { get; init; }
    public Guid CategoryId { get; init; }
    public CategoryDto Category { get; init; } = null!;
}

public record CreateCoffeeItemRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public string? ImageUrl { get; init; }
}

public record UpdateCoffeeItemRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public Guid CategoryId { get; init; }
    public string? ImageUrl { get; init; }
}
