using DatingApp.Core.Domain.IdentityEntities;
using DatingApp.Core.DTO;
using DatingApp.Core.ServiceContracts;
using DatingApp.Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly Jwt _Jwt;
        private readonly UserManager<AppUser> _userManager;
        public AuthService(IOptions<Jwt> jwtOptions, UserManager<AppUser> userManager)
        {
            _Jwt = jwtOptions.Value;
            _userManager = userManager;
        }
        public async Task<AuthenticationUserDTO> CreateJwtToken(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));


            var claims = new Claim[] {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //Subject (user id)
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //JWT unique ID
              new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64), //Issued at (date and time of token generation)
              new Claim(ClaimTypes.Email, user.Email), //Unique name identifier of the user (Email)
              new Claim(ClaimTypes.Name, user.DisplayName), //Name of the user
              
            }
            .Union(roleClaims);
            
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddMinutes(_Jwt.DurationInMinutes);          
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
            _Jwt.Issuer,
            _Jwt.Audience,
            claims,
            expires: expiration,
            signingCredentials: signingCredentials
            );
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);
            return new AuthenticationUserDTO() {
                Token = token,
                Id = user.Id,
                Email = user?.Email,
                DisplayName = user?.DisplayName,
                Expiration = expiration,
                IsAuthenticated = true,
                ImageUrl = user?.ImageUrl
            };
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
