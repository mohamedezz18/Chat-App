using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Helpers
{
    public class MemberParams : PagingParams
    {
        public enum OrderType { created , LastActive }
        public string? Gender { get; set; }
        public Guid? CurrentMemberId { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;

        public OrderType OrderBy { get; set; } = OrderType.LastActive;
    }
}
