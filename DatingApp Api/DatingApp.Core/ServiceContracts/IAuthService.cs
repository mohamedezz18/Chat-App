using DatingApp.Core.Domain.IdentityEntities;
using DatingApp.Core.DTO;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Core.ServiceContracts
{
    public interface IAuthService
    {
        Task<AuthenticationUserDTO> CreateJwtToken(AppUser user);
        string GenerateRefreshToken();
        
    }
}
