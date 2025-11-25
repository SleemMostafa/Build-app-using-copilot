using CoffeeRestaurant.Domain.Events;

namespace CoffeeRestaurant.Domain.Entities;

public class Order : BaseEntity
{
    // Private setters to enforce business rules
    public Guid CustomerId { get; private set; }
    public Guid? BaristaId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal TotalPrice { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Barista? Barista { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    // Private constructor for EF Core
    private Order() { }

    /// <summary>
    /// Factory method to create a new order
    /// </summary>
    public static Order Create(Guid customerId, IEnumerable<OrderItem> orderItems, string? notes = null)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        var items = orderItems?.ToList() ?? throw new ArgumentNullException(nameof(orderItems));
        if (!items.Any())
            throw new ArgumentException("Order must have at least one item", nameof(orderItems));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Notes = notes,
            OrderItems = items
        };

        // Calculate total price
        order.TotalPrice = items.Sum(i => i.UnitPrice * i.Quantity);

        // Raise domain event
        order.AddDomainEvent(new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = customerId,
            TotalPrice = order.TotalPrice,
            ItemCount = items.Count
        });

        return order;
    }

    /// <summary>
    /// Assign a barista to this order
    /// </summary>
    public void AssignBarista(Guid baristaId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Can only assign barista to pending orders");

        BaristaId = baristaId;
        ChangeStatus(OrderStatus.InProgress);
    }

    /// <summary>
    /// Change the status of the order
    /// </summary>
    public void ChangeStatus(OrderStatus newStatus)
    {
        if (Status == newStatus)
            return;

        ValidateStatusTransition(newStatus);

        var previousStatus = Status;
        Status = newStatus;

        AddDomainEvent(new OrderStatusChangedEvent
        {
            OrderId = Id,
            PreviousStatus = previousStatus.ToString(),
            NewStatus = newStatus.ToString()
        });

        if (newStatus == OrderStatus.Completed)
        {
            AddDomainEvent(new OrderCompletedEvent
            {
                OrderId = Id,
                CustomerId = CustomerId,
                TotalPrice = TotalPrice
            });
        }
    }

    /// <summary>
    /// Cancel the order
    /// </summary>
    public void Cancel(string? reason = null)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed order");

        if (Status == OrderStatus.Cancelled)
            return;

        Status = OrderStatus.Cancelled;

        AddDomainEvent(new OrderCancelledEvent
        {
            OrderId = Id,
            Reason = reason
        });
    }

    /// <summary>
    /// Mark order as ready for pickup
    /// </summary>
    public void MarkAsReady()
    {
        if (Status != OrderStatus.InProgress)
            throw new InvalidOperationException("Only in-progress orders can be marked as ready");

        ChangeStatus(OrderStatus.Ready);
    }

    /// <summary>
    /// Complete the order
    /// </summary>
    public void Complete()
    {
        if (Status != OrderStatus.Ready)
            throw new InvalidOperationException("Only ready orders can be completed");

        ChangeStatus(OrderStatus.Completed);
    }

    /// <summary>
    /// Recalculate total price based on order items
    /// </summary>
    public void RecalculateTotalPrice()
    {
        TotalPrice = OrderItems.Sum(i => i.UnitPrice * i.Quantity);
    }

    private void ValidateStatusTransition(OrderStatus newStatus)
    {
        var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
        {
            { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.InProgress, OrderStatus.Cancelled } },
            { OrderStatus.InProgress, new List<OrderStatus> { OrderStatus.Ready, OrderStatus.Cancelled } },
            { OrderStatus.Ready, new List<OrderStatus> { OrderStatus.Completed, OrderStatus.Cancelled } },
            { OrderStatus.Completed, new List<OrderStatus>() },
            { OrderStatus.Cancelled, new List<OrderStatus>() }
        };

        if (!validTransitions[Status].Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition from {Status} to {newStatus}");
        }
    }
}

public enum OrderStatus
{
    Pending,
    InProgress,
    Ready,
    Completed,
    Cancelled
}
