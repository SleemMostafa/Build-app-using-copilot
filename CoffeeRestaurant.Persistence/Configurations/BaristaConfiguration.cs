using CoffeeRestaurant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeRestaurant.Persistence.Configurations;

public class BaristaConfiguration : IEntityTypeConfiguration<Barista>
{
    public void Configure(EntityTypeBuilder<Barista> builder)
    {
        builder.ToTable("Baristas");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.UserId)
            .IsRequired()
            .HasMaxLength(450);
            
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.HasIndex(b => b.UserId)
            .IsUnique();
    }
}
