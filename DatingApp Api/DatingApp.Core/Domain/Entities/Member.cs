using DatingApp.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DatingApp.Core.Domain.Entities
{
    public class Member
    {
        [Required]
        public Guid Id { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }

        [Required]
        public  string? DisplayName { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        [Required]
        public string? Gender { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Country { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual AppUser User { get; set; } = null!;

        public List<Photo> Photos { get; set; } = [];

        [JsonIgnore]
        public List<MemberLike> LikedByMembers { get; set; } = [];
        [JsonIgnore]
        public List<MemberLike> LikedMembers { get; set; } = [];

        [JsonIgnore]
        public List<Message> MessagesSent { get; set; } = [];

        [JsonIgnore]
        public List<Message> MessagesRecieved { get; set; } = [];
    }
}
