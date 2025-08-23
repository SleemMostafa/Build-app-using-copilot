using CoffeeRestaurant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeRestaurant.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<CoffeeItem> CoffeeItems { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Barista> Baristas { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
