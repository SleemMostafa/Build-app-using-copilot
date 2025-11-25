using CoffeeRestaurant.Domain.Common;

namespace CoffeeRestaurant.Domain.Events;

/// <summary>
/// Domain event raised when a new order is created
/// </summary>
public record OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalPrice { get; init; }
    public int ItemCount { get; init; }
}

/// <summary>
/// Domain event raised when an order status changes
/// </summary>
public record OrderStatusChangedEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public string PreviousStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
}

/// <summary>
/// Domain event raised when an order is completed
/// </summary>
public record OrderCompletedEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalPrice { get; init; }
}

/// <summary>
/// Domain event raised when an order is cancelled
/// </summary>
public record OrderCancelledEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public string? Reason { get; init; }
}
