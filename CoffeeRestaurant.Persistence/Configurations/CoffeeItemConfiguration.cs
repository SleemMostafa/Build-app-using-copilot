using CoffeeRestaurant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeRestaurant.Persistence.Configurations;

public class CoffeeItemConfiguration : IEntityTypeConfiguration<CoffeeItem>
{
    public void Configure(EntityTypeBuilder<CoffeeItem> builder)
    {
        builder.ToTable("CoffeeItems");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(c => c.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);
            
        builder.HasOne(c => c.Category)
            .WithMany(c => c.CoffeeItems)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(c => c.Name);
    }
}
