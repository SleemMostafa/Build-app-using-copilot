using Microsoft.AspNetCore.Identity;

namespace CoffeeRestaurant.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Customer? Customer { get; set; }
    public virtual Barista? Barista { get; set; }
}
