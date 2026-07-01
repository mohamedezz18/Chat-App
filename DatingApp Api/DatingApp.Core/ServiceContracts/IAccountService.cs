using DatingApp.Core.Domain.IdentityEntities;
using DatingApp.Core.DTO;

namespace DatingApp.Core.ServiceContracts
{
    public interface IAccountService
    {
        Task<AuthResult> RegisterAsync(RegisterDto model);
        Task<AuthResult> LoginAsync(LoginDto model);
        Task<bool> LogoutAsync(Guid userId);
        Task<bool> SetRefreshTokenForUser(AppUser user);
        Task<AuthResult> RefreshTokenAsync(string token);

        Task<bool> EmailIsExists(string email);
        Task<(bool success, string message)> EditRoleForUser(Guid id , IList<string> roleNames);
        Task<IReadOnlyList<UserWithRoleDto>> GetUsersWithRolesAsync();
    }
}
