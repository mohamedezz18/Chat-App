using API.Controllers;
using DatingApp.Core.DTO;
using DatingApp.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp.UI.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _accountService.EmailIsExists(model.Email))
            {
                return BadRequest(new
                {
                    message = "Email is already registered",
                    code = "EMAIL_EXISTS"
                });
            }

            var result = await _accountService.RegisterAsync(model);
            if (result == null || result.Success == false)
            {
                return BadRequest(new
                {
                    message = "Registration failed",
                    errors = result?.Errors,
                    code = result?.ErrorCode
                });
            }

            if (result?.AuthenticationUser?.IsAuthenticated == false)
            {
                return BadRequest(new
                {
                    message = "Authentication failed",
                    code = "AUTHENTICATION_FAILED"
                });
            }
            SetRefreshTokenInCookie(result?.AuthenticationUser?.RefreshToken, result?.AuthenticationUser?.RefreshTokenExpiration);
            return Ok(result?.AuthenticationUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var result = await _accountService.LoginAsync(model);

            if (result == null || result.Success == false)
            {

                return Unauthorized(new
                {
                    message = "Invalid email or password",
                    errors = result?.Errors,
                });
            }
            SetRefreshTokenInCookie(result.AuthenticationUser?.RefreshToken, result.AuthenticationUser?.RefreshTokenExpiration);

            return Ok(result?.AuthenticationUser);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null) return NoContent();

            var result = await _accountService.RefreshTokenAsync(refreshToken);

            if (result == null || result.Success == false)
            {
                return Unauthorized(new
                {
                    errors = result?.Errors
                });
            }

            SetRefreshTokenInCookie(result.AuthenticationUser?.RefreshToken, result.AuthenticationUser?.RefreshTokenExpiration);
            return Ok(result.AuthenticationUser);
        }


        private void SetRefreshTokenInCookie(string? refreshToken, DateTime? expires)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return;
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Expires = expires
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.GetMemberId();
            var result = await _accountService.LogoutAsync(userId);
            if (!result)
            {
                return BadRequest(new
                {
                    message = "Logout failed"
                });
            }
            Response.Cookies.Delete("refreshToken");
            return Ok(new
            {
                message = "Logged out successfully"
            });
        }

    }
}
