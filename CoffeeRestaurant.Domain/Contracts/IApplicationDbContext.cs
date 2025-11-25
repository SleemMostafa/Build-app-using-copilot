using CoffeeRestaurant.Domain.Entities;

namespace CoffeeRestaurant.Domain.Contracts;

/// <summary>
/// Database context contract that uses IQueryable for data access.
/// This approach keeps the domain layer free from specific ORM dependencies
/// while allowing LINQ queries in the Application layer.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets queryable collection of Categories
    /// </summary>
    IQueryable<Category> Categories { get; }
    
    /// <summary>
    /// Gets queryable collection of Coffee Items
    /// </summary>
    IQueryable<CoffeeItem> CoffeeItems { get; }
    
    /// <summary>
    /// Gets queryable collection of Customers
    /// </summary>
    IQueryable<Customer> Customers { get; }
    
    /// <summary>
    /// Gets queryable collection of Baristas
    /// </summary>
    IQueryable<Barista> Baristas { get; }
    
    /// <summary>
    /// Gets queryable collection of Orders
    /// </summary>
    IQueryable<Order> Orders { get; }
    
    /// <summary>
    /// Gets queryable collection of Order Items
    /// </summary>
    IQueryable<OrderItem> OrderItems { get; }
    
    /// <summary>
    /// Adds an entity to the context
    /// </summary>
    void Add<TEntity>(TEntity entity) where TEntity : class;
    
    /// <summary>
    /// Updates an entity in the context
    /// </summary>
    void Update<TEntity>(TEntity entity) where TEntity : class;
    
    /// <summary>
    /// Removes an entity from the context
    /// </summary>
    void Remove<TEntity>(TEntity entity) where TEntity : class;
    
    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

