using CoffeeRestaurant.Domain.Contracts;
using CoffeeRestaurant.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoffeeRestaurant.Persistence.Context;

public class CoffeeDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public CoffeeDbContext(DbContextOptions<CoffeeDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<CoffeeItem> CoffeeItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Barista> Baristas { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    // Explicit interface implementation for IApplicationDbContext
    IQueryable<Category> IApplicationDbContext.Categories => Categories;
    IQueryable<CoffeeItem> IApplicationDbContext.CoffeeItems => CoffeeItems;
    IQueryable<Customer> IApplicationDbContext.Customers => Customers;
    IQueryable<Barista> IApplicationDbContext.Baristas => Baristas;
    IQueryable<Order> IApplicationDbContext.Orders => Orders;
    IQueryable<OrderItem> IApplicationDbContext.OrderItems => OrderItems;

    void IApplicationDbContext.Add<TEntity>(TEntity entity) => Set<TEntity>().Add(entity);
    void IApplicationDbContext.Update<TEntity>(TEntity entity) => Set<TEntity>().Update(entity);
    void IApplicationDbContext.Remove<TEntity>(TEntity entity) => Set<TEntity>().Remove(entity);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply configurations
        builder.ApplyConfigurationsFromAssembly(typeof(CoffeeDbContext).Assembly);
        
        // Configure Identity
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            // Clear domain events after processing (handled by domain event dispatcher)
            // entry.Entity.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
