using CoffeeRestaurant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeRestaurant.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.OrderDate)
            .IsRequired();
            
        builder.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(o => o.Notes)
            .HasMaxLength(1000);
            
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(o => o.Barista)
            .WithMany(b => b.Orders)
            .HasForeignKey(o => o.BaristaId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.Status);
    }
}
