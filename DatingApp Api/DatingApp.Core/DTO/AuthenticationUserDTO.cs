using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DatingApp.Core.DTO
{
    public class AuthenticationUserDTO
    {
        public Guid Id { get; set; }
        public string ? DisplayName { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Token { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; } = false;
        public string? ImageUrl { get; set; }
        public DateTime Expiration { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
