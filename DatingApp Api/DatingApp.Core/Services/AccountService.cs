using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.IdentityEntities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.DTO;
using DatingApp.Core.ServiceContracts;
using DatingApp.Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;


namespace DatingApp.Core.Services
{
    public class AccountService : IAccountService
    {
        public readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        private readonly Jwt _jwt;

        public AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IOptions<Jwt> jwt , IAuthService authService,
            RoleManager<IdentityRole<Guid>> roleManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt.Value;
            _authService = authService;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        public async Task<bool> EmailIsExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto model)
        {
           
            AppUser user = new AppUser
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email,
                Member = new Member
                {
                    DisplayName = model.DisplayName,
                    Gender = model.Gender,
                    City = model.City,
                    Country = model.Country,
                    DateOfBirth = model.DateOfBirth,
                }
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                await _userManager.AddToRoleAsync(user, "Member");
                await _signInManager.SignInAsync(user, isPersistent: false);

                AuthenticationUserDTO authenticationResponse =await _authService.CreateJwtToken(user);
                if (await SetRefreshTokenForUser(user))
                {
                    authenticationResponse.RefreshToken = user.RefreshToken;
                    authenticationResponse.RefreshTokenExpiration = user.RefreshTokenExpiry;
                }
                return  new AuthResult
                {
                    Success = true,

                    AuthenticationUser = authenticationResponse
                }; 
            }

            else
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorCode = "VALIDATION_ERROR",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }
        }

        public async Task<AuthResult> LoginAsync(LoginDto model)
        {
            if (model == null)
            {
                return new AuthResult { Success = false,
                    ErrorCode = "INVALID_REQUEST",
                    Errors = new List<string> { "Invalid login request." } };
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorCode = "INVALID_CREDENTIALS",
                    Errors = new List<string> { "Invalid email or password" }
                };
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            // technically rare but safe check
            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorCode = "USER_NOT_FOUND",
                    Errors = new List<string> { "Invalid email or password" }
                };
            }

            var _authenticationUser = await _authService.CreateJwtToken(user);

            if (user.RefreshToken == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                if (await SetRefreshTokenForUser(user))
                {
                    _authenticationUser.RefreshToken = user.RefreshToken;
                    _authenticationUser.RefreshTokenExpiration = user.RefreshTokenExpiry;
                }
            }
            else
            {
                _authenticationUser.RefreshToken = user.RefreshToken;
                _authenticationUser.RefreshTokenExpiration = user.RefreshTokenExpiry;
            }

            return new AuthResult
            {
                Success = true,
                AuthenticationUser = _authenticationUser
            };
        }

        public async Task<(bool success, string message)> EditRoleForUser(Guid id, IList<string> roleNames)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null )
                return (false, "Could not retrieve user");

            var currentRoles = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roleNames)
            {
                
                    if (!await _roleManager.RoleExistsAsync(roleName))
                        return (false, $"Role '{roleName}' does not exist");

            }

            var rolesToAdd = roleNames.Except(currentRoles).ToList();

             var rolesToRemove = currentRoles.Except(roleNames).ToList();


            if (rolesToAdd.Any())
            {
                var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!result.Succeeded)
                {
                    return (false, string.Join(", \n", result.Errors.Select(e => e.Description)));
                }
            }


            if (rolesToRemove.Any())
            {

                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                if (!removeResult.Succeeded)
                {
                    return (
                        false,
                        string.Join(", ", removeResult.Errors.Select(e => e.Description))
                    );
                }
            }
            var AllRoles = await _userManager.GetRolesAsync(user);
            return (true, string.Join(",", AllRoles));
        }

        public async Task<IReadOnlyList<UserWithRoleDto>> GetUsersWithRolesAsync()
        {
            return await _userRepository.GetUsersWithRolesAsync();
        }

        public async Task<bool> SetRefreshTokenForUser(AppUser user)
        {
            var refreshToken = _authService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDurationInDays);
            var result =  await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<AuthResult> RefreshTokenAsync(string token)
        {

            AuthResult authResult = new AuthResult();
            AppUser? user = await _userManager.Users
             .FirstOrDefaultAsync(x => x.RefreshToken == token 
                && x.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                authResult.Success = false;
                authResult.ErrorCode = "INVALID_REFRESH_TOKEN";
                authResult.Errors = new List<string> { "Invalid refresh token" };
                return authResult;

            }

            if (await SetRefreshTokenForUser(user))
            {
                authResult.Success = true;
                authResult.AuthenticationUser = await _authService.CreateJwtToken(user);
                authResult.AuthenticationUser.RefreshTokenExpiration = user.RefreshTokenExpiry;
                authResult.AuthenticationUser.RefreshToken = user.RefreshToken;
            }
            return authResult;
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return false;

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
