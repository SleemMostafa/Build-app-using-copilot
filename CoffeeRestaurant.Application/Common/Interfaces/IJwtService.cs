using CoffeeRestaurant.Domain.Entities;

namespace CoffeeRestaurant.Application.Common.Interfaces;

/// <summary>
/// JWT token service interface.
/// This belongs to Application layer as it's an application concern for authentication.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user and roles
    /// </summary>
    string GenerateToken(ApplicationUser user, IList<string> roles);
    
    /// <summary>
    /// Validates a JWT token and returns claims principal
    /// </summary>
    System.Security.Claims.ClaimsPrincipal? ValidateToken(string token);
}
