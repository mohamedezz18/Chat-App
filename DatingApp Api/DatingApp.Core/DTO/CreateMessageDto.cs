using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatingApp.Core.DTO
{
    public class CreateMessageDto
    {
        [Required]
        public Guid RecipientId { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
