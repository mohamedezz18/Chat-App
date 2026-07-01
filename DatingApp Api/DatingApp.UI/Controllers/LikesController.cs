using API.Controllers;
using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Helpers;
using DatingApp.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DatingApp.Core.Domain.RepositoryContracts.ILikesRepository;

namespace DatingApp.UI.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly ILikesService _likesService;

        public LikesController(ILikesService likesService)
        {
            _likesService = likesService;
        }

        [HttpPost("{targetMemberID}")]
        public async Task<ActionResult> ToggleLike(Guid targetMemberID)
        {
            var sourceMemberId = User.GetMemberId();

            if (sourceMemberId == targetMemberID)
            {
                return BadRequest("you cannot like yourself");
            }

            if(await _likesService.AddToggleLikeAsync(sourceMemberId, targetMemberID))
            {
                return Ok();
            }
            return BadRequest("Failed To Like");
        }

        [HttpGet("lists")]
        public async Task<ActionResult<IReadOnlyList<Guid>>> GetCurrentMembersLikeIds()
        {
            return Ok(await _likesService.GetCurrentMemberLikeIdsAsync(User.GetMemberId()));
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes(
            [FromQuery]LikesParams likesParams)
        {
            likesParams.MemberId = User.GetMemberId();
            var members = await _likesService.GetMemberLikesAsync(likesParams);

            return Ok(members);
        }
    }
}
