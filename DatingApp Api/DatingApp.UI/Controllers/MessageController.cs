using API.Controllers;
using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using DatingApp.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.UI.Controllers
{
    public class MessagesController : BaseApiController
    {
        public readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var SenderID = User.GetMemberId();

            if (SenderID == createMessageDto.RecipientId)
            {
                return BadRequest("Cannot Send Message To Yourself");
            }

            var messageDto = await _messageService.CreateMessageAsync(createMessageDto, SenderID);
            if (messageDto != null)
            {
                return Ok(messageDto);
            }
            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer(
        [FromQuery] MessageParams messageParams)
        {
            messageParams.MemberId = User.GetMemberId();

            return await _messageService.GetMessagesForMemberAsync(messageParams);
        }


        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(Guid recipientId)
        {
            var currentMemberId = User.GetMemberId();
            return Ok(await _messageService.GetMessageThreadAsync(currentMemberId, recipientId));

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(Guid id)
        {
            if (await _messageService.DeleteMessageAsync(User.GetMemberId(), id))
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the message");
        }
    }
}
