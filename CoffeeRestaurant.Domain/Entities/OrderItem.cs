namespace CoffeeRestaurant.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid CoffeeItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string? SpecialInstructions { get; set; }
    
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual CoffeeItem CoffeeItem { get; set; } = null!;
}
