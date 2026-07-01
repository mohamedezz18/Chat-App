using DatingApp.Core.Domain.Entities;
using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Domain.RepositoryContracts
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message?> GetMessage(Guid messageId);
        Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams);
        Task<IReadOnlyList<MessageDto>> GetMessageThread(Guid currentMemberId, Guid recipientId);
        Task<bool> SaveAllChangesAsync();
    }
}
