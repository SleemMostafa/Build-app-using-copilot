namespace CoffeeRestaurant.Shared.DTOs;

public class OrderDto : BaseDto
{
    public Guid CustomerId { get; init; }
    public Guid? BaristaId { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalPrice { get; init; }
    public OrderStatus Status { get; init; }
    public string? Notes { get; init; }
    public CustomerDto Customer { get; init; } = null!;
    public BaristaDto? Barista { get; init; }
    public List<OrderItemDto> OrderItems { get; init; } = new();
}

public class OrderItemDto : BaseDto
{
    public Guid OrderId { get; init; }
    public Guid CoffeeItemId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
    public string? SpecialInstructions { get; init; }
    public CoffeeItemDto CoffeeItem { get; init; } = null!;
}

public class CustomerDto : BaseDto
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
}

public class BaristaDto : BaseDto
{
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public record CreateOrderRequest
{
    public Guid CustomerId { get; init; }
    public List<CreateOrderItemRequest> OrderItems { get; init; } = new();
    public string? Notes { get; init; }
}

public record CreateOrderItemRequest
{
    public Guid CoffeeItemId { get; init; }
    public int Quantity { get; init; }
    public string? SpecialInstructions { get; init; }
}

public record UpdateOrderStatusRequest
{
    public OrderStatus Status { get; init; }
    public string? Notes { get; init; }
}

public enum OrderStatus
{
    Pending,
    InProgress,
    Ready,
    Completed,
    Cancelled
}
