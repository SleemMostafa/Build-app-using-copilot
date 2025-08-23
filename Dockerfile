FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CoffeeRestaurant.Api/CoffeeRestaurant.Api.csproj", "CoffeeRestaurant.Api/"]
COPY ["CoffeeRestaurant.Application/CoffeeRestaurant.Application.csproj", "CoffeeRestaurant.Application/"]
COPY ["CoffeeRestaurant.Domain/CoffeeRestaurant.Domain.csproj", "CoffeeRestaurant.Domain/"]
COPY ["CoffeeRestaurant.Infrastructure/CoffeeRestaurant.Infrastructure.csproj", "CoffeeRestaurant.Infrastructure/"]
COPY ["CoffeeRestaurant.Persistence/CoffeeRestaurant.Persistence.csproj", "CoffeeRestaurant.Persistence/"]
COPY ["CoffeeRestaurant.Shared/CoffeeRestaurant.Shared.csproj", "CoffeeRestaurant.Shared/"]
RUN dotnet restore "CoffeeRestaurant.Api/CoffeeRestaurant.Api.csproj"
COPY . .
WORKDIR "/src/CoffeeRestaurant.Api"
RUN dotnet build "CoffeeRestaurant.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoffeeRestaurant.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeRestaurant.Api.dll"]
