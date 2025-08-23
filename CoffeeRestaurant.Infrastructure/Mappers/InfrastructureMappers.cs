using CoffeeRestaurant.Domain.Entities;
using CoffeeRestaurant.Shared.DTOs;

namespace CoffeeRestaurant.Infrastructure.Mappers;

public static class InfrastructureMappers
{
    // JWT service mappings
    public static LoginResponse ToLoginResponse(this ApplicationUser user, string token, string refreshToken, DateTime expiresAt, List<string> roles)
    {
        return new LoginResponse(
            token,
            refreshToken,
            expiresAt,
            user.ToUserDto(roles)
        );
    }

    // User service mappings
    public static UserDto ToUserDto(this ApplicationUser user, List<string> roles)
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

    // Email service mappings (if you implement email functionality)
    public static EmailDto ToEmailDto(this ApplicationUser user, string subject, string body)
    {
        return new EmailDto
        {
            To = user.Email ?? string.Empty,
            Subject = subject,
            Body = body,
            UserName = $"{user.FirstName} {user.LastName}"
        };
    }

    // External service mappings (if you integrate with third-party services)
    public static ExternalServiceDto ToExternalServiceDto(this CoffeeItem coffeeItem)
    {
        return new ExternalServiceDto
        {
            ItemId = coffeeItem.Id.ToString(),
            Name = coffeeItem.Name,
            Description = coffeeItem.Description,
            Price = coffeeItem.Price,
            Category = coffeeItem.Category?.Name ?? "Unknown"
        };
    }
}

// Placeholder DTOs for infrastructure services
public class EmailDto
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class ExternalServiceDto
{
    public string ItemId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

