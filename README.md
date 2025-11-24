# Coffee Restaurant API

A .NET 10 Web API project for a coffee restaurant built using Clean Architecture principles.

## 🏗️ Architecture

This project follows Clean Architecture with the following layers:

- **CoffeeRestaurant.Api** - API Layer (Controllers, Middleware)
- **CoffeeRestaurant.Application** - Application Layer (CQRS, MediatR, Validators)
- **CoffeeRestaurant.Domain** - Domain Layer (Entities, Business Logic)
- **CoffeeRestaurant.Infrastructure** - Infrastructure Layer (External Services, JWT)
- **CoffeeRestaurant.Persistence** - Data Access Layer (EF Core, DbContext)
- **CoffeeRestaurant.Shared** - Shared Kernel (DTOs, Common Utilities)

## 🚀 Features

- **ASP.NET Core Identity** with JWT Bearer authentication
- **CQRS Pattern** using MediatR
- **Response Classes** - each command/query defines its own response type
- **FluentValidation** for request validation
- **Entity Framework Core** with SQL Server
- **EF Core Projections** for optimized query performance
- **Swagger/OpenAPI** documentation
- **Docker** support
- **Role-based Authorization** (Admin, Barista, Customer)

## 📋 Prerequisites

- .NET 10 SDK
- SQL Server (LocalDB or SQL Server Express)
- Docker (optional, for containerized deployment)

## 🛠️ Setup Instructions

### Option 1: Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CoffeeRestaurant
   ```

2. **Update Connection String**
   Edit `CoffeeRestaurant.Api/appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CoffeeRestaurantDb;Trusted_Connection=true;MultipleActiveResultSets=true"
   }
   ```

3. **Update JWT Secret**
   In `CoffeeRestaurant.Api/appsettings.json`, update the JWT secret:
   ```json
   "Jwt": {
     "Secret": "your-super-secret-key-with-at-least-32-characters"
   }
   ```

4. **Run the application**
   ```bash
   cd CoffeeRestaurant.Api
   dotnet run
   ```

5. **Access the API**
   - Swagger UI: https://localhost:7001/swagger
   - API Base URL: https://localhost:7001/api

### Option 2: Docker

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

2. **Access the application**
   - Swagger UI: http://localhost:5000/swagger
   - API Base URL: http://localhost:5000/api

## 🔐 Authentication

### Default Admin User
- **Email**: admin@coffeerestaurant.com
- **Password**: Admin123!

### User Registration
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890"
}
```

### User Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `GET /api/auth/me` - Get current user info (requires authentication)

### Coffee Items
- `GET /api/coffeeitems` - Get all coffee items
- `POST /api/coffeeitems` - Create coffee item (Admin only)

### Orders
- `GET /api/orders` - Get all orders (requires authentication)
- `POST /api/orders` - Create new order (requires authentication)

## 🗄️ Database Schema

### Entities
- **ApplicationUser** - Identity user with custom properties
- **Category** - Coffee categories (Espresso, Latte, etc.)
- **CoffeeItem** - Menu items with prices
- **Customer** - Customer information
- **Barista** - Barista information
- **Order** - Customer orders
- **OrderItem** - Individual items in orders

### Seeded Data
The application automatically seeds:
- Default roles (Admin, Barista, Customer)
- Coffee categories
- Sample coffee items
- Admin user

## 🔧 Configuration

### JWT Settings
```json
{
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "CoffeeRestaurant",
    "Audience": "CoffeeRestaurant",
    "ExpiryInHours": 24
  }
}
```

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=CoffeeRestaurantDb;..."
  }
}
```

## 🧪 Testing

### Using Swagger UI
1. Navigate to `/swagger`
2. Use the "Try it out" feature to test endpoints
3. For protected endpoints, first login to get a JWT token
4. Click "Authorize" and enter: `Bearer <your-jwt-token>`

### Using curl
```bash
# Register
curl -X POST "https://localhost:7001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Password123!","firstName":"Test","lastName":"User"}'

# Login
curl -X POST "https://localhost:7001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Password123!"}'

# Get coffee items
curl -X GET "https://localhost:7001/api/coffeeitems"
```

## 🐳 Docker Commands

```bash
# Build and run
docker-compose up --build

# Run in background
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f api
```

## 📝 Project Structure

```
CoffeeRestaurant/
├── CoffeeRestaurant.Api/           # API Layer
│   ├── Controllers/               # API Controllers
│   ├── Program.cs                 # Application entry point
│   └── appsettings.json          # Configuration
├── CoffeeRestaurant.Application/   # Application Layer
│   ├── Common/                   # Common interfaces & behaviors
│   ├── CoffeeItems/              # Coffee items CQRS
│   └── Orders/                   # Orders CQRS
├── CoffeeRestaurant.Domain/       # Domain Layer
│   └── Entities/                 # Domain entities
├── CoffeeRestaurant.Infrastructure/ # Infrastructure Layer
│   └── Services/                 # External services
├── CoffeeRestaurant.Persistence/  # Data Access Layer
│   ├── Context/                  # DbContext
│   ├── Configurations/           # EF configurations
│   └── DataSeeder.cs            # Data seeding
├── CoffeeRestaurant.Shared/       # Shared Layer
│   ├── DTOs/                    # Data transfer objects
│   └── Common/                  # Shared utilities
├── Dockerfile                    # Docker configuration
├── docker-compose.yml           # Docker Compose
└── README.md                    # This file
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.
