using CoffeeRestaurant.Domain.Entities;
using CoffeeRestaurant.Shared.DTOs;

namespace CoffeeRestaurant.Persistence.Mappers;

public static class PersistenceMappers
{
    // Database context mappings
    public static CoffeeItemDto ToDtoWithCategory(this CoffeeItem entity, Category category)
    {
        return new CoffeeItemDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            IsAvailable = entity.IsAvailable,
            ImageUrl = entity.ImageUrl,
            CategoryId = entity.CategoryId,
            Category = category.ToDto(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // Order with related entities mappings
    public static OrderDto ToDtoWithRelations(this Order entity, Customer customer, Barista? barista, List<OrderItem> orderItems)
    {
        return new OrderDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            BaristaId = entity.BaristaId,
            OrderDate = entity.OrderDate,
            TotalPrice = entity.TotalPrice,
            Status = entity.Status,
            Notes = entity.Notes,
            Customer = customer.ToDto(),
            Barista = barista?.ToDto(),
            OrderItems = orderItems.Select(oi => oi.ToDto()).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // OrderItem with CoffeeItem mapping
    public static OrderItemDto ToDtoWithCoffeeItem(this OrderItem entity, CoffeeItem coffeeItem)
    {
        return new OrderItemDto
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            CoffeeItemId = entity.CoffeeItemId,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice,
            Subtotal = entity.Subtotal,
            SpecialInstructions = entity.SpecialInstructions,
            CoffeeItem = coffeeItem.ToDto(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // Customer with ApplicationUser mapping
    public static CustomerDto ToDtoWithUser(this Customer entity, ApplicationUser user)
    {
        return new CustomerDto
        {
            Id = entity.Id,
            Name = user.FirstName + " " + user.LastName,
            Email = user.Email ?? string.Empty,
            Phone = user.PhoneNumber,
            Address = entity.Address,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // Barista with ApplicationUser mapping
    public static BaristaDto ToDtoWithUser(this Barista entity, ApplicationUser user)
    {
        return new BaristaDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = user.FirstName + " " + user.LastName,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // List mappings with eager loading
    public static List<CoffeeItemDto> ToDtoListWithCategories(this IEnumerable<CoffeeItem> entities, Dictionary<Guid, Category> categories)
    {
        return entities.Select(e => e.ToDtoWithCategory(categories.GetValueOrDefault(e.CategoryId, new Category()))).ToList();
    }

    public static List<OrderDto> ToDtoListWithRelations(this IEnumerable<Order> entities, 
        Dictionary<Guid, Customer> customers, 
        Dictionary<Guid, Barista> baristas, 
        Dictionary<Guid, List<OrderItem>> orderItems)
    {
        return entities.Select(e => e.ToDtoWithRelations(
            customers.GetValueOrDefault(e.CustomerId, new Customer()),
            baristas.GetValueOrDefault(e.BaristaId ?? Guid.Empty),
            orderItems.GetValueOrDefault(e.Id, new List<OrderItem>())
        )).ToList();
    }
}

