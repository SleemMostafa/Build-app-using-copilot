# Coffee Restaurant API - AI Agent Guide

## Architecture Overview

This is a **.NET 10 Web API** following **Clean Architecture** with strict layer separation:

- **Api** → Controllers, Program.cs (entry point)
- **Application** → CQRS handlers (Commands/Queries), validators, behaviors, **response classes**
- **Domain** → Entities (inherit from `BaseEntity`), business logic
- **Infrastructure** → External services (JWT, email, etc.)
- **Persistence** → DbContext, EF Core configurations, data seeding
- **Shared** → `ApiResponse<T>`, common utilities

**Critical**: Dependencies flow inward: Api → Application → Domain. Never reference outer layers from inner layers.

## Response Classes (NO DTOs or Mappers)

**This project uses inline response classes** - each command/query defines its own response type at the end of the file.

### Pattern
```csharp
// Command with response
public record CreateCoffeeItemCommand : IRequest<CreateCoffeeItemResponse> { ... }

public class CreateCoffeeItemCommandHandler : IRequestHandler<CreateCoffeeItemCommand, CreateCoffeeItemResponse>
{
    public async Task<CreateCoffeeItemResponse> Handle(...)
    {
        var entity = new CoffeeItem { ... };
        await _context.SaveChangesAsync(cancellationToken);
        
        return new CreateCoffeeItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price
        };
    }
}

// Response class at end of same file
public record CreateCoffeeItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
```

### EF Core Projections
For queries, use `.Select()` to project directly to response:
```csharp
return await _context.CoffeeItems
    .Select(ci => new GetCoffeeItemsResponse
    {
        Id = ci.Id,
        Name = ci.Name,
        CategoryName = ci.Category.Name
    })
    .ToListAsync(cancellationToken);
```

**When adding new operations**: Define response class at the end of command/query file.

## CQRS Pattern with MediatR

All business logic flows through MediatR commands/queries:

### Structure Pattern
```
Application/
  {Feature}/
    Commands/
      {CommandName}/
        {CommandName}Command.cs        # Record implementing IRequest<TResponse>
        {CommandName}CommandHandler.cs # IRequestHandler implementation
        {CommandName}CommandValidator.cs # FluentValidation (if needed)
    Queries/
      {QueryName}/
        {QueryName}Query.cs
        {QueryName}QueryHandler.cs
```

### Example
```csharp
// Command definition
public record CreateCoffeeItemCommand : IRequest<CoffeeItemDto> { ... }

// Handler
public class CreateCoffeeItemCommandHandler : IRequestHandler<CreateCoffeeItemCommand, CoffeeItemDto>
{
    private readonly IApplicationDbContext _context;
    
    public async Task<CoffeeItemDto> Handle(CreateCoffeeItemCommand request, ...)
    {
        // Business logic
        var entity = new CoffeeItem { ... };
        _context.CoffeeItems.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.ToDto(); // Manual mapping
    }
}
```

### Controllers
Controllers are thin - they inject `IMediator` and send commands/queries:
```csharp
var result = await _mediator.Send(new CreateCoffeeItemCommand { ... }, cancellationToken);
return Ok(ApiResponse<CoffeeItemDto>.SuccessResult(result));
```

## API Response Pattern

**All endpoints return `ApiResponse<T>`** from `Shared/Common/ApiResponse.cs`:

```csharp
// Success
return Ok(ApiResponse<UserDto>.SuccessResult(userDto, "User retrieved"));

// Error
return BadRequest(ApiResponse<UserDto>.ErrorResult("Not found", errors));
```

Properties: `bool Success`, `string? Message`, `T? Data`, `List<string>? Errors`

## Validation

**FluentValidation** runs automatically via `ValidationBehavior<TRequest, TResponse>` pipeline registered in `Program.cs`.

- Validators live next to commands: `{CommandName}CommandValidator.cs`
- Extend `AbstractValidator<TCommand>`
- Validation failures throw `ValidationException` before handler executes

Example: `CreateCoffeeItemCommandValidator` validates Name, Description, Price, CategoryId.

## Authentication & Authorization

- **ASP.NET Core Identity** with `ApplicationUser : IdentityUser`
- **JWT Bearer** tokens (configured in `Program.cs`)
- Roles: `Admin`, `Barista`, `Customer` (seeded in `DataSeeder.cs`)
- Default admin: `admin@coffeerestaurant.com` / `Admin123!`

### JWT Service
`IJwtService` (Infrastructure layer) generates tokens. Controllers use `UserManager<ApplicationUser>` and `SignInManager<ApplicationUser>`.

### Authorization
```csharp
[Authorize] // Requires authentication
[Authorize(Roles = "Admin")] // Requires specific role
```

## Database & EF Core

- **SQL Server** with EF Core
- DbContext: `CoffeeDbContext` in Persistence, implements `IApplicationDbContext`
- Entities inherit from `BaseEntity` (Id, CreatedAt, UpdatedAt)
- Configurations: `Persistence/Configurations/{Entity}Configuration.cs` using `IEntityTypeConfiguration<T>`

### Data Seeding
`DataSeeder.cs` runs on app startup (see `Program.cs`):
- Roles: Admin, Barista, Customer
- Categories: Espresso, Cappuccino, Latte, etc.
- Sample coffee items
- Admin user

### Connection Strings
- Local: `appsettings.json` uses `(localdb)\\mssqllocaldb`
- Docker: `docker-compose.yml` overrides with `Server=db;...`

## Development Workflow

### Build & Run
```bash
# Local (from CoffeeRestaurant.Api/)
dotnet run

# Docker (from solution root)
docker-compose up --build
```

### Ports
- Local: HTTPS `7001`, HTTP `5000`
- Docker: HTTP `5000`, HTTPS `5001`
- Swagger: `/swagger` (dev only)

### Testing API
1. Register/login to get JWT token
2. In Swagger: Click "Authorize" → Enter `Bearer {token}`
3. Test endpoints

### Database Migrations
```bash
# From solution root
dotnet ef migrations add MigrationName --project CoffeeRestaurant.Persistence --startup-project CoffeeRestaurant.Api
dotnet ef database update --project CoffeeRestaurant.Persistence --startup-project CoffeeRestaurant.Api
```

## Key Conventions

1. **Use records for DTOs and Commands/Queries** (immutable)
2. **DateTime.UtcNow** for timestamps (never local time)
3. **Guid.NewGuid()** for entity IDs
4. **Async/await** everywhere (all handlers are async)
5. **CancellationToken** passed to all async operations
6. **Explicit null handling** (`string.Empty` defaults, null-conditional operators)

## Common Pitfalls

- ❌ Don't create separate DTO files - use response classes in command/query files
- ❌ Don't create mapper classes - construct responses directly
- ❌ Don't inject DbContext directly in Application layer - use `IApplicationDbContext`
- ❌ Don't put business logic in controllers - use command handlers
- ❌ Don't forget validators for new commands
- ❌ Don't reference Application/Infrastructure from Domain
- ❌ Don't load full entities then map - use EF Core projections with `.Select()`

## Adding New Features

1. Create entity in `Domain/Entities/` (inherit `BaseEntity`)
2. Add DbSet to `IApplicationDbContext` and `CoffeeDbContext`
3. Create EF configuration in `Persistence/Configurations/`
4. Create CQRS structure in `Application/{Feature}/Commands|Queries/`
5. Define response classes at the end of each command/query file
6. Use EF Core projections (`.Select()`) for queries
7. Add validators for commands
8. Create controller in `Api/Controllers/`
9. Register services if needed in `Program.cs`

## Resources

- See `README.md` for setup instructions
- See `MAPPING_GUIDE.md` for detailed response class patterns
- Swagger UI available at `/swagger` in development mode
