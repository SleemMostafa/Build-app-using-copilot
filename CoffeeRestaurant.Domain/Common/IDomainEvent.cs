namespace CoffeeRestaurant.Domain.Common;

/// <summary>
/// Base interface for domain events.
/// Domain events represent something that happened in the domain that domain experts care about.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

/// <summary>
/// Base abstract class for domain events with timestamp.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
