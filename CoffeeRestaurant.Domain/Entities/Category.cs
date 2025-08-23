namespace CoffeeRestaurant.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation properties
    public virtual ICollection<CoffeeItem> CoffeeItems { get; set; } = new List<CoffeeItem>();
}
