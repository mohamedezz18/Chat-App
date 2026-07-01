using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace DatingApp.Core.Domain.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        public string? PublicId { get; set; }

        // Navigation property
        [JsonIgnore]
        public virtual Member Member { get; set; } = null!;
        public Guid MemberId { get; set; } 
    }
}
