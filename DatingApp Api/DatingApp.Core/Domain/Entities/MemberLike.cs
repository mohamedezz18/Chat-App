using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatingApp.Core.Domain.Entities
{
    public class MemberLike
    {
        [Required]
        public  Guid SourceMemberID {  get; set; }
        public Member SourceMember { get; set; }
        [Required]
        public Guid TargetMemberID { get; set; }
        public Member TargetMember { get; set; }

    }
}
