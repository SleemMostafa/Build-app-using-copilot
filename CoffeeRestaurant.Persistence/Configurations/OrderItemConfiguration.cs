using CoffeeRestaurant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeRestaurant.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        
        builder.HasKey(oi => oi.Id);
        
        builder.Property(oi => oi.Quantity)
            .IsRequired();
            
        builder.Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(oi => oi.Subtotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(oi => oi.SpecialInstructions)
            .HasMaxLength(200);
            
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(oi => oi.CoffeeItem)
            .WithMany(ci => ci.OrderItems)
            .HasForeignKey(oi => oi.CoffeeItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
