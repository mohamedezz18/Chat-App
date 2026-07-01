using API.Controllers;
using DatingApp.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.UI.Controllers
{

    public class AdminController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AdminController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult<IList<string>>> EditRoles(Guid userId, [FromQuery] string roles)
        {
            IList<string> rolesList = roles.Split(',').Select(r => r.Trim()).ToList();
            var result = await _accountService.EditRoleForUser(userId, rolesList);
           if(result.success == false)
           {
               return BadRequest(result.message);
           }

           return Ok(result.message.Split(",").ToList());
        }



        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            return Ok(await _accountService.GetUsersWithRolesAsync());
        }
    }
}
