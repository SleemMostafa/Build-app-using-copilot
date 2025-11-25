namespace CoffeeRestaurant.Domain.Contracts;

/// <summary>
/// Interface for services that need to access current user information.
/// This belongs to Domain layer as it's a domain concern.
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
