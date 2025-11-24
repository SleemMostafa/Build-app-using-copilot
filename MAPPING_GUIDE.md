# Response Classes Guide

This guide explains the response-based architecture used in the Coffee Restaurant API.

## 🎯 Overview

Instead of using DTOs and mappers, we now use **dedicated response classes** for each command and query. This approach provides:
- **Better Encapsulation**: Each operation has its own response type
- **Type Safety**: Compile-time checking with specific response contracts
- **Clearer Intent**: Response structure matches the operation's purpose
- **No Mapping Overhead**: Direct object construction
- **Self-Documenting**: Response class lives with the command/query

## 🏗️ Architecture

Response classes are defined **at the end of each command/query file**:

```
Application/
├── CoffeeItems/
│   ├── Commands/
│   │   └── CreateCoffeeItem/
│   │       ├── CreateCoffeeItemCommand.cs
│   │       ├── CreateCoffeeItemCommandHandler.cs
│   │       └── CreateCoffeeItemResponse (at end of file)
│   └── Queries/
│       └── GetCoffeeItems/
│           ├── GetCoffeeItemsQuery.cs
│           ├── GetCoffeeItemsQueryHandler.cs
│           └── GetCoffeeItemsResponse (at end of file)
```

## 📚 Usage Examples

### 1. Command with Response

```csharp
// Command definition
public record CreateCoffeeItemCommand : IRequest<CreateCoffeeItemResponse>
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    // ... other properties
}

// Handler
public class CreateCoffeeItemCommandHandler : IRequestHandler<CreateCoffeeItemCommand, CreateCoffeeItemResponse>
{
    public async Task<CreateCoffeeItemResponse> Handle(...)
    {
        var entity = new CoffeeItem { ... };
        _context.CoffeeItems.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Direct response construction
        return new CreateCoffeeItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            CreatedAt = entity.CreatedAt
        };
    }
}

// Response class at end of file
public record CreateCoffeeItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

### 2. Query with Response

```csharp
// Query definition
public record GetCoffeeItemsQuery : IRequest<List<GetCoffeeItemsResponse>>;

// Handler with projection
public class GetCoffeeItemsQueryHandler : IRequestHandler<GetCoffeeItemsQuery, List<GetCoffeeItemsResponse>>
{
    public async Task<List<GetCoffeeItemsResponse>> Handle(...)
    {
        // Direct EF Core projection to response
        return await _context.CoffeeItems
            .Include(ci => ci.Category)
            .Where(ci => ci.IsAvailable)
            .Select(ci => new GetCoffeeItemsResponse
            {
                Id = ci.Id,
                Name = ci.Name,
                CategoryName = ci.Category.Name,
                // ... other properties
            })
            .ToListAsync(cancellationToken);
    }
}

// Response class at end of file
public record GetCoffeeItemsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
}
```

### 3. Nested Response Classes

```csharp
public record CreateOrderResponse
{
    public Guid Id { get; init; }
    public decimal TotalPrice { get; init; }
    public List<CreateOrderItemResponse> OrderItems { get; init; } = new();
}

public record CreateOrderItemResponse
{
    public Guid Id { get; init; }
    public string CoffeeItemName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Subtotal { get; init; }
}
```

## 🚀 Benefits

### Before (with DTOs and Mappers)
```csharp
// Scattered across multiple files
var dto = coffeeItem.ToDto(); // Mapper in separate file
return dto; // DTO in Shared layer
```

### After (with Response Classes)
```csharp
// All in one place
return new CreateCoffeeItemResponse
{
    Id = coffeeItem.Id,
    Name = coffeeItem.Name,
    Price = coffeeItem.Price
};
```

**Advantages:**
- No external mapping layer needed
- Response structure is immediately visible
- Easy to customize per operation
- Better performance (direct construction)
- Simpler debugging (no extension methods to trace)

## 📝 Best Practices

### 1. Response Class Naming
```csharp
// Good: Operation-specific names
CreateCoffeeItemResponse
GetCoffeeItemsResponse
UpdateOrderStatusResponse

// Avoid: Generic names
CoffeeItemDto
ItemResponse
```

### 2. Use Records
```csharp
// Good: Immutable records
public record GetCoffeeItemsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

// Avoid: Mutable classes
public class GetCoffeeItemsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

### 3. EF Core Projections
```csharp
// Good: Direct projection in query
.Select(ci => new GetCoffeeItemsResponse
{
    Id = ci.Id,
    CategoryName = ci.Category.Name
})

// Avoid: Loading full entity then mapping
var items = await _context.CoffeeItems.ToListAsync();
return items.Select(i => new Response { ... });
```

### 4. Flat Response Structures
```csharp
// Good: Flat structure with necessary data
public record GetOrdersResponse
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
}

// Avoid: Nested complex objects when not needed
public record GetOrdersResponse
{
    public Guid Id { get; init; }
    public CustomerDto Customer { get; init; } // Overkill if only name needed
}
```

### 5. Response Location
Always place response classes **at the end of the same file** as the command/query:

```csharp
// CreateCoffeeItemCommand.cs
public record CreateCoffeeItemCommand : IRequest<CreateCoffeeItemResponse> { ... }
public class CreateCoffeeItemCommandHandler { ... }

// At the end of the same file
public record CreateCoffeeItemResponse { ... }
```

## 🔧 Controller Usage

Controllers reference response types directly:

```csharp
[HttpPost]
public async Task<ActionResult<ApiResponse<CreateCoffeeItemResponse>>> CreateCoffeeItem(CreateCoffeeItemCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(ApiResponse<CreateCoffeeItemResponse>.SuccessResult(result));
}
```

## 🧪 Testing

Response classes are easy to test:

```csharp
[Test]
public async Task CreateCoffeeItem_ShouldReturnResponse()
{
    // Arrange
    var command = new CreateCoffeeItemCommand { Name = "Latte", Price = 4.50m };
    
    // Act
    var response = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    response.Name.Should().Be("Latte");
    response.Price.Should().Be(4.50m);
}
```

## 📊 Comparison

| Aspect | DTOs + Mappers | Response Classes |
|--------|----------------|------------------|
| **Files** | 3+ (DTO, Mapper, Command) | 1 (Command + Response) |
| **Maintenance** | Update multiple files | Update single file |
| **Performance** | Reflection/mapping overhead | Direct construction |
| **Clarity** | Scattered across layers | Co-located with operation |
| **Type Safety** | Runtime mapping errors | Compile-time validation |
| **Customization** | Shared DTO for all ops | Unique per operation |

## 🎉 Conclusion

The response class pattern provides:
- **Simplicity**: One file per operation
- **Performance**: Direct object construction with EF Core projections
- **Maintainability**: Co-located request and response
- **Flexibility**: Custom response per operation
- **Type Safety**: Full compile-time checking

This architecture is production-ready and scales well with the application's growth!

## 🎯 Overview

Instead of using AutoMapper, we now use extension methods for manual mapping. This approach provides:
- **Better Performance**: No reflection overhead
- **Type Safety**: Compile-time checking
- **Explicit Control**: Clear mapping logic
- **Easier Debugging**: Step-through mapping code
- **No External Dependencies**: Built-in .NET functionality

## 🏗️ Architecture

The mapping system is organized by layers:

```
CoffeeRestaurant/
├── Domain/Mappers/           # Core entity mappings
├── Application/Mappers/      # Command/Query mappings
├── Infrastructure/Mappers/   # Service mappings
├── Persistence/Mappers/      # Database mappings
└── Shared/Mappers/          # Common utility mappings
```

## 📚 Usage Examples

### 1. Basic Entity to DTO Mapping

```csharp
// Before (AutoMapper)
var dto = _mapper.Map<CoffeeItemDto>(coffeeItem);

// After (Manual Mapping)
var dto = coffeeItem.ToDto();
```

### 2. List Mapping

```csharp
// Before (AutoMapper)
var dtos = _mapper.Map<List<CoffeeItemDto>>(coffeeItems);

// After (Manual Mapping)
var dtos = coffeeItems.ToDtoList();
```

### 3. Command to DTO Mapping

```csharp
// Before (AutoMapper)
var dto = _mapper.Map<CoffeeItemDto>(command);

// After (Manual Mapping)
var dto = command.ToDto(coffeeItem.Id, coffeeItem.CreatedAt);
```

### 4. Complex Mapping with Relations

```csharp
// Before (AutoMapper)
var orderDto = _mapper.Map<OrderDto>(order);

// After (Manual Mapping)
var orderDto = order.ToDtoWithRelations(customer, barista, orderItems);
```

## 🔧 Available Mappers

### Domain Layer Mappers (`CoffeeRestaurant.Domain.Mappers`)

```csharp
// CoffeeItem mappings
coffeeItem.ToDto()                                    // Basic mapping
coffeeItem.ToEntity(createRequest)                    // DTO to entity
coffeeItem.UpdateFromDto(updateRequest)               // Update entity

// Order mappings
order.ToDto()                                         // Basic mapping
order.ToEntity(createRequest)                         // DTO to entity

// Category mappings
category.ToDto()                                      // Basic mapping

// Customer mappings
customer.ToDto()                                      // Basic mapping

// Barista mappings
barista.ToDto()                                       // Basic mapping
```

### Application Layer Mappers (`CoffeeRestaurant.Application.Mappers`)

```csharp
// Command mappings
command.ToDto(id, createdAt, totalPrice)              // Command to DTO

// User mappings
user.ToDto(roles)                                     // User with roles

// List mappings
entities.ToDtoList()                                   // Generic list mapping
```

### Infrastructure Layer Mappers (`CoffeeRestaurant.Infrastructure.Mappers`)

```csharp
// Authentication mappings
user.ToLoginResponse(token, refreshToken, expiresAt, roles)
user.ToUserDto(roles)

// Service mappings
user.ToEmailDto(subject, body)
coffeeItem.ToExternalServiceDto()
```

### Persistence Layer Mappers (`CoffeeRestaurant.Persistence.Mappers`)

```csharp
// Database mappings with relations
coffeeItem.ToDtoWithCategory(category)
order.ToDtoWithRelations(customer, barista, orderItems)
orderItem.ToDtoWithCoffeeItem(coffeeItem)
customer.ToDtoWithUser(user)
barista.ToDtoWithUser(user)

// Eager loading mappings
entities.ToDtoListWithCategories(categories)
entities.ToDtoListWithRelations(customers, baristas, orderItems)
```

### Shared Layer Mappers (`CoffeeRestaurant.Shared.Mappers`)

```csharp
// Generic mappings
entities.ToDtoList<TEntity, TDto>(mapper)

// Pagination mappings
entities.ToPagedResult(mapper, pageNumber, pageSize, totalCount)

// Response mappings
data.ToSuccessResponse(message)
error.ToErrorResponse(errors)

// Validation mappings
propertyName.ToValidationError(errorMessage)
validationErrors.ToValidationErrors()

// Search mappings
entities.ToSearchResult(mapper, searchTerm, totalCount)
```

## 🚀 Performance Benefits

### Before (AutoMapper)
```csharp
// Reflection-based mapping
var dto = _mapper.Map<CoffeeItemDto>(entity);
```

### After (Manual Mapping)
```csharp
// Direct property assignment
var dto = entity.ToDto();
```

**Performance Improvement**: 2-5x faster execution time due to:
- No reflection overhead
- Direct property access
- Compile-time optimization
- No dynamic method generation

## 🔍 Debugging

### AutoMapper Issues
- Runtime mapping errors
- Difficult to debug mapping logic
- Configuration complexity
- Performance bottlenecks

### Manual Mapping Benefits
- Compile-time checking
- Easy to step through code
- Clear mapping logic
- Predictable performance

## 📝 Best Practices

### 1. Keep Mappers Simple
```csharp
// Good: Simple, focused mapping
public static CoffeeItemDto ToDto(this CoffeeItem entity)
{
    return new CoffeeItemDto
    {
        Id = entity.Id,
        Name = entity.Name,
        // ... other properties
    };
}

// Avoid: Complex logic in mappers
public static CoffeeItemDto ToDto(this CoffeeItem entity)
{
    // Don't put business logic here
    if (entity.Price > 100) { /* ... */ }
    return new CoffeeItemDto { /* ... */ };
}
```

### 2. Use Extension Methods
```csharp
// Good: Extension method syntax
var dto = entity.ToDto();

// Avoid: Static method calls
var dto = DomainMappers.ToDto(entity);
```

### 3. Handle Null Values
```csharp
// Good: Safe navigation
Category = entity.Category?.ToDto() ?? new CategoryDto()

// Avoid: Potential null reference exceptions
Category = entity.Category.ToDto()
```

### 4. Use Appropriate Mappers
```csharp
// For basic mapping
var dto = entity.ToDto();

// For complex relations
var dto = entity.ToDtoWithRelations(customer, barista, orderItems);

// For lists
var dtos = entities.ToDtoList();
```

## 🔄 Migration from AutoMapper

### Step 1: Remove AutoMapper References
```xml
<!-- Remove from .csproj files -->
<PackageReference Include="AutoMapper" Version="15.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
```

### Step 2: Remove Service Registration
```csharp
// Remove from Program.cs
builder.Services.AddAutoMapper(typeof(Startup).Assembly);
```

### Step 3: Replace Mapping Calls
```csharp
// Before
var dto = _mapper.Map<CoffeeItemDto>(entity);

// After
var dto = entity.ToDto();
```

### Step 4: Add Using Statements
```csharp
using CoffeeRestaurant.Domain.Mappers;
using CoffeeRestaurant.Application.Mappers;
using CoffeeRestaurant.Infrastructure.Mappers;
using CoffeeRestaurant.Persistence.Mappers;
using CoffeeRestaurant.Shared.Mappers;
```

## 🧪 Testing

### Unit Testing Mappers
```csharp
[Test]
public void ToDto_ShouldMapAllProperties()
{
    // Arrange
    var entity = new CoffeeItem
    {
        Id = Guid.NewGuid(),
        Name = "Test Coffee",
        Price = 5.99m
    };

    // Act
    var dto = entity.ToDto();

    // Assert
    dto.Id.Should().Be(entity.Id);
    dto.Name.Should().Be(entity.Name);
    dto.Price.Should().Be(entity.Price);
}
```

## 📊 Comparison

| Aspect | AutoMapper | Manual Mapping |
|--------|------------|----------------|
| **Performance** | Slower (reflection) | Faster (direct) |
| **Type Safety** | Runtime checking | Compile-time |
| **Debugging** | Difficult | Easy |
| **Dependencies** | External package | Built-in |
| **Configuration** | Complex | Simple |
| **Maintenance** | Harder | Easier |
| **Learning Curve** | Steep | Gentle |

## 🎉 Conclusion

The manual mapping approach provides:
- **Better Performance**: 2-5x faster execution
- **Improved Maintainability**: Clear, explicit code
- **Enhanced Debugging**: Easy to troubleshoot
- **Reduced Dependencies**: No external packages
- **Type Safety**: Compile-time validation

This system is production-ready and provides a solid foundation for the Coffee Restaurant API.

