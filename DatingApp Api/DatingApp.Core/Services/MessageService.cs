using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using DatingApp.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository messageRepository;
        private readonly IMembersService membersService;

        public MessageService(IMessageRepository messageRepository, IMembersService membersService)
        {
            this.messageRepository = messageRepository;
            this.membersService = membersService;
        }

        public async Task<MessageDto> CreateMessageAndReadAsync(CreateMessageDto createMessageDto, Guid SenderID)
        {

            Member? sender = await this.membersService.GetMemberByID(SenderID);
            Member? Recipient = await this.membersService.GetMemberByID(createMessageDto.RecipientId);

            if (sender == null && Recipient == null)
            {
                return null!;
            }

            Message message = new Message()
            {
                SenderId = SenderID,
                RecipientId = createMessageDto.RecipientId,
                Content = createMessageDto.Content,
                Sender = sender,
                Recipient = Recipient,
                DateRead = DateTime.Now
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllChangesAsync()) return message.ToDto();

            return null!;

        }

        public async Task<MessageDto> CreateMessageAsync(CreateMessageDto createMessageDto, Guid SenderID)
        { 
            Member? sender =await this.membersService.GetMemberByID(SenderID);
            Member? Recipient =await this.membersService.GetMemberByID(createMessageDto.RecipientId);

            if (sender == null && Recipient == null)
            {
                return null!;
            }

            Message message = new Message()
            {
                SenderId = SenderID,
                RecipientId = createMessageDto.RecipientId,
                Content = createMessageDto.Content,
                Sender = sender,
                Recipient = Recipient
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllChangesAsync()) return message.ToDto();

            return null!;
            

        }

        public async Task<bool> DeleteMessageAsync(Guid currentMemberId, Guid Id)
        {
            Message? message = await messageRepository.GetMessage(Id);
            if (message == null)
                return false;

            if (message.SenderId != currentMemberId && message.RecipientId != currentMemberId)
                return false;

            if (message.SenderId == currentMemberId) message.SenderDeleted = true;
            if (message.RecipientId == currentMemberId) message.RecipientDeleted = true;

            if (message is { SenderDeleted: true, RecipientDeleted: true })
            {
                messageRepository.DeleteMessage(message);
            }

            return await messageRepository.SaveAllChangesAsync();

        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMemberAsync(MessageParams messageParams)
        {

            if (messageParams == null)
                throw new ArgumentNullException(nameof(messageParams));


            return await messageRepository.GetMessagesForMember(messageParams);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThreadAsync(Guid currentMemberId, Guid recipientId)
        {
            
            if(currentMemberId == Guid.Empty && recipientId == Guid.Empty)
            {
                return null!;
            }

           return await messageRepository.GetMessageThread(currentMemberId, recipientId);
        }
    }
}
