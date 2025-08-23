using CoffeeRestaurant.Domain.Entities;
using CoffeeRestaurant.Infrastructure.Services;
using CoffeeRestaurant.Shared.Common;
using CoffeeRestaurant.Shared.DTOs;
using CoffeeRestaurant.Infrastructure.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeRestaurant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("User with this email already exists."));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Registration failed.", errors));
        }

        // Add to Customer role by default
        await _userManager.AddToRoleAsync(user, "Customer");

        // Generate token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        var response = user.ToLoginResponse(
            token,
            "refresh-token-placeholder", // In a real app, implement refresh tokens
            DateTime.UtcNow.AddHours(24),
            roles.ToList()
        );

        return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Invalid email or password."));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Invalid email or password."));
        }

        if (!user.IsActive)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Account is deactivated."));
        }

        // Generate token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        var response = user.ToLoginResponse(
            token,
            "refresh-token-placeholder", // In a real app, implement refresh tokens
            DateTime.UtcNow.AddHours(24),
            roles.ToList()
        );

        return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Login successful."));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<UserDto>.ErrorResult("User not authenticated."));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDto>.ErrorResult("User not found."));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            roles.ToList()
        );

        return Ok(ApiResponse<UserDto>.SuccessResult(userDto));
    }
}
