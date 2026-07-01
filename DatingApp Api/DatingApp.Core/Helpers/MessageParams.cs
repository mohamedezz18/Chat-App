using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Helpers
{
    public class MessageParams : PagingParams
    {
        public enum MessageContainer { Inbox, Outbox }

        public Guid MemberId { get; set; }
        public MessageContainer Container { get; set; } = MessageContainer.Inbox;
    }
}
