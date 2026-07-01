using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.DTO
{
    public class UserWithRoleDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
