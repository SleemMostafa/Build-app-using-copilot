using CoffeeRestaurant.Domain.Events;

namespace CoffeeRestaurant.Domain.Entities;

public class CoffeeItem : BaseEntity
{
    // Private setters to enforce business rules
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public string? ImageUrl { get; set; }
    
    // Foreign key
    public Guid CategoryId { get; private set; }
    
    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // Private constructor for EF Core
    private CoffeeItem() { }

    /// <summary>
    /// Factory method to create a new coffee item
    /// </summary>
    public static CoffeeItem Create(string name, string description, decimal price, Guid categoryId, string? imageUrl = null)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID cannot be empty", nameof(categoryId));

        var coffeeItem = new CoffeeItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            CategoryId = categoryId,
            ImageUrl = imageUrl,
            IsAvailable = true
        };

        coffeeItem.AddDomainEvent(new CoffeeItemCreatedEvent
        {
            CoffeeItemId = coffeeItem.Id,
            Name = name,
            Price = price
        });

        return coffeeItem;
    }

    /// <summary>
    /// Update coffee item details
    /// </summary>
    public void UpdateDetails(string name, string description, decimal price)
    {
        ValidateName(name);
        ValidateDescription(description);

        var priceChanged = Price != price;
        var oldPrice = Price;

        Name = name;
        Description = description;

        if (priceChanged)
        {
            ValidatePrice(price);
            Price = price;
            
            AddDomainEvent(new CoffeeItemPriceChangedEvent
            {
                CoffeeItemId = Id,
                OldPrice = oldPrice,
                NewPrice = price
            });
        }
    }

    /// <summary>
    /// Change the price of the coffee item
    /// </summary>
    public void ChangePrice(decimal newPrice)
    {
        ValidatePrice(newPrice);

        if (Price == newPrice)
            return;

        var oldPrice = Price;
        Price = newPrice;

        AddDomainEvent(new CoffeeItemPriceChangedEvent
        {
            CoffeeItemId = Id,
            OldPrice = oldPrice,
            NewPrice = newPrice
        });
    }

    /// <summary>
    /// Set availability status
    /// </summary>
    public void SetAvailability(bool isAvailable)
    {
        if (IsAvailable == isAvailable)
            return;

        IsAvailable = isAvailable;

        AddDomainEvent(new CoffeeItemAvailabilityChangedEvent
        {
            CoffeeItemId = Id,
            IsAvailable = isAvailable
        });
    }

    /// <summary>
    /// Mark item as available
    /// </summary>
    public void MarkAsAvailable() => SetAvailability(true);

    /// <summary>
    /// Mark item as unavailable
    /// </summary>
    public void MarkAsUnavailable() => SetAvailability(false);

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters", nameof(name));
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        if (price > 10000)
            throw new ArgumentException("Price cannot exceed 10,000", nameof(price));
    }
}
