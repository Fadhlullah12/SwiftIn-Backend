using Microsoft.AspNetCore.Identity;

namespace SwiftIn.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
            public string FullName { get; set; } = default!;
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}