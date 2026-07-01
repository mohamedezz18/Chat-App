using System.ComponentModel.DataAnnotations;

namespace DatingApp.Core.DTO
{
    public class MessageDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid SenderId { get; set; }
        [Required]
        public string SenderDisplayName { get; set; }
        public string? SenderImageUrl { get; set; }
        [Required]
        public Guid RecipientId { get; set; }
        [Required]
        public string RecipientDisplayName { get; set; }
        public string? RecipientImageUrl { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}
