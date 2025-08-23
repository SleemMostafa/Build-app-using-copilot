namespace CoffeeRestaurant.Shared.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string? PhoneNumber);
public record LoginResponse(string Token, string RefreshToken, DateTime ExpiresAt, UserDto User);
public record UserDto(string Id, string Email, string FirstName, string LastName, string? PhoneNumber, List<string> Roles);
public record RefreshTokenRequest(string Token, string RefreshToken);
