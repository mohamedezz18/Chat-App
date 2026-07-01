using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.DTO
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public List<string>? Errors { get; set; }
        public AuthenticationUserDTO? AuthenticationUser { get; set; }
    }
}
