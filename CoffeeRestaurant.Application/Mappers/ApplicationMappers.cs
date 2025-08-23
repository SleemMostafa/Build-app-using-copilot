using CoffeeRestaurant.Domain.Entities;
using CoffeeRestaurant.Shared.DTOs;

namespace CoffeeRestaurant.Application.Mappers;

public static class ApplicationMappers
{
    // CoffeeItem mappings
    public static CoffeeItemDto ToDto(this CoffeeItem entity)
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
            Category = entity.Category?.ToDto() ?? new CategoryDto(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static CoffeeItem ToEntity(this CreateCoffeeItemRequest dto)
    {
        return new CoffeeItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFromDto(this CoffeeItem entity, UpdateCoffeeItemRequest dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.IsAvailable = dto.IsAvailable;
        entity.CategoryId = dto.CategoryId;
        entity.ImageUrl = dto.ImageUrl;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    // Category mappings
    public static CategoryDto ToDto(this Category entity)
    {
        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // Order mappings
    public static OrderDto ToDto(this Order entity)
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
            Customer = entity.Customer?.ToDto() ?? new CustomerDto(),
            Barista = entity.Barista?.ToDto(),
            OrderItems = entity.OrderItems?.Select(oi => oi.ToDto()).ToList() ?? new List<OrderItemDto>(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Order ToEntity(this CreateOrderRequest dto)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Notes = dto.Notes,
            TotalPrice = 0, // Will be calculated
            CreatedAt = DateTime.UtcNow
        };
    }

    // OrderItem mappings
    public static OrderItemDto ToDto(this OrderItem entity)
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
            CoffeeItem = entity.CoffeeItem?.ToDto() ?? new CoffeeItemDto(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static OrderItem ToEntity(this CreateOrderItemRequest dto, Guid orderId, decimal unitPrice)
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            CoffeeItemId = dto.CoffeeItemId,
            Quantity = dto.Quantity,
            UnitPrice = unitPrice,
            Subtotal = dto.Quantity * unitPrice,
            SpecialInstructions = dto.SpecialInstructions,
            CreatedAt = DateTime.UtcNow
        };
    }

    // Customer mappings
    public static CustomerDto ToDto(this Customer entity)
    {
        return new CustomerDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            Address = entity.Address,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // Barista mappings
    public static BaristaDto ToDto(this Barista entity)
    {
        return new BaristaDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // Command mappings
    public static CoffeeItemDto ToDto(this CreateCoffeeItemCommand command, Guid id, DateTime createdAt)
    {
        return new CoffeeItemDto
        {
            Id = id,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            CategoryId = command.CategoryId,
            ImageUrl = command.ImageUrl,
            IsAvailable = true,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
    }

    // User mappings for authentication
    public static UserDto ToDto(this ApplicationUser user, List<string> roles)
    {
        return new UserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            roles
        );
    }

    // List mappings
    public static List<CoffeeItemDto> ToDtoList(this IEnumerable<CoffeeItem> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    public static List<OrderDto> ToDtoList(this IEnumerable<Order> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    public static List<CategoryDto> ToDtoList(this IEnumerable<Category> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}
