using CoffeeRestaurant.Domain.Common;

namespace CoffeeRestaurant.Domain.Entities;

/// <summary>
/// Enhanced base entity with domain events support.
/// All entities should inherit from this class to get common properties and domain event functionality.
/// </summary>
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Timestamp when the entity was created (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Timestamp when the entity was last updated (UTC)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Domain events that have occurred on this entity
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Add a domain event to this entity
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Remove a specific domain event
    /// </summary>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clear all domain events
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
