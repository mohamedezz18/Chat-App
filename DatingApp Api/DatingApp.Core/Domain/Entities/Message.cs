using DatingApp.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text;

namespace DatingApp.Core.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }

        // nav properties
        [Required]
        public Guid SenderId { get; set; }
        public Member Sender { get; set; } = null!;
        [Required]
        public Guid RecipientId { get; set; }
        public Member Recipient { get; set; } = null!;
    }



    public static class MessageExtensions
    {
        public static MessageDto ToDto(this Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderDisplayName = message.Sender.DisplayName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientDisplayName = message.Recipient.DisplayName,
                RecipientImageUrl = message.Recipient.ImageUrl,
                Content = message.Content,
                DateRead = message.DateRead,
                MessageSent = message.MessageSent
            };
        }

        public static Expression<Func<Message, MessageDto>> ToDtoProjection()
        {
            return message => new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderDisplayName = message.Sender.DisplayName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientDisplayName = message.Recipient.DisplayName,
                RecipientImageUrl = message.Recipient.ImageUrl,
                Content = message.Content,
                DateRead = message.DateRead,
                MessageSent = message.MessageSent
            };
        }
    }
}
