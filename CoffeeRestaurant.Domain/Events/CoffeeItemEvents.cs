using CoffeeRestaurant.Domain.Common;

namespace CoffeeRestaurant.Domain.Events;

/// <summary>
/// Domain event raised when a new coffee item is added
/// </summary>
public record CoffeeItemCreatedEvent : DomainEvent
{
    public Guid CoffeeItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

/// <summary>
/// Domain event raised when coffee item availability changes
/// </summary>
public record CoffeeItemAvailabilityChangedEvent : DomainEvent
{
    public Guid CoffeeItemId { get; init; }
    public bool IsAvailable { get; init; }
}

/// <summary>
/// Domain event raised when coffee item price changes
/// </summary>
public record CoffeeItemPriceChangedEvent : DomainEvent
{
    public Guid CoffeeItemId { get; init; }
    public decimal OldPrice { get; init; }
    public decimal NewPrice { get; init; }
}
