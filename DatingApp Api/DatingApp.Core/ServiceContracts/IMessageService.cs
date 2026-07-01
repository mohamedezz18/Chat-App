using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.ServiceContracts
{
    public interface IMessageService
    {
        Task<MessageDto> CreateMessageAsync(CreateMessageDto createMessageDto, Guid SenderID);
        Task<MessageDto> CreateMessageAndReadAsync(CreateMessageDto createMessageDto, Guid SenderID);
        Task<PaginatedResult<MessageDto>> GetMessagesForMemberAsync(MessageParams messageParams);

        Task<IReadOnlyList<MessageDto>> GetMessageThreadAsync(Guid currentMemberId, Guid recipientId);
        Task<bool> DeleteMessageAsync(Guid currentMemberId, Guid MessageId);


    }
}
