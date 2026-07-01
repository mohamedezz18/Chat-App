using API.Controllers;
using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using DatingApp.Core.ServiceContracts;
using DatingApp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp.UI.Controllers
{

    [Authorize]
    public class MembersController : BaseApiController
    {
        private readonly IMembersService _memberService;
        private readonly IPhotoService _photoService;


        public MembersController(IMembersService memberService , IPhotoService PhotoService)
        {
            _memberService = memberService;
            _photoService = PhotoService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers(
            [FromQuery]MemberParams memberParams )
        {
            memberParams.CurrentMemberId = User.GetMemberId();

            return Ok(await _memberService.GetMembers(memberParams));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(Guid id)
        {
            var member = await _memberService.GetMemberByID(id);

            if (member == null) return NotFound();

            return member;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(Guid id)
        {
            return Ok(await _memberService.GetPhotosForMember(id));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(UpdateMemberDto updateMemberDto)
        {
            var memberId = User.GetMemberId();

            var result = await _memberService.UpdateMember(memberId, updateMemberDto);
            if (result.Success == true)
            {
               return  NoContent();
            }
             return BadRequest(result.Message);
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm]IFormFile file )
        {
            Member? member =await _memberService.GetMemberWithNAvigationByID(User.GetMemberId());
            if (member == null) return BadRequest("cannot update");


            var result  = await _photoService.UploadPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            Photo photo = new Photo()
            { 
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };

            if (member.ImageUrl == null) 
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }

           if (await _memberService.AddPhotoForMember(member, photo))
           {
               return Ok(photo);
           }

           return BadRequest("Failed Adding Photo");

        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {

            Member? member = await _memberService.GetMemberWithNAvigationByID(User.GetMemberId());
            if (member == null) return BadRequest("can get member");

            Photo? photo = member.Photos.SingleOrDefault(p => p.Id == photoId);
             
            if (member.ImageUrl == photo?.Url || photo == null) 
            {
                return BadRequest("Cannot set this a main photo");

            }

            if (await _memberService.SetMainPhotoForMember(member, photo))
            { 
                return NoContent();
            }

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            Member? member = await _memberService.GetMemberWithNAvigationByID(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");

            Photo? photo = member.Photos.SingleOrDefault(p => p.Id == photoId);

            if (photo == null || photo.Url == member.ImageUrl)
            {
                return BadRequest("This photo cannot be deleted");
            }

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            if (await _memberService.RemovePhotoForMember(member, photo)) 
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the photo");
        }
    }
}
