using DatingApp.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace DatingApp.Core.Domain.IdentityEntities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = "";
        public string? ImageUrl { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public virtual Member Member { get; set; } = null!;
    }
}
