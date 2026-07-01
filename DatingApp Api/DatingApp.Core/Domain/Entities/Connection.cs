using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DatingApp.Core.Domain.Entities
{
    public class Connection
    {
        
        public string ConnectionId { get; set; } 
        public string UserId { get; set; } 

        // nav property
        public Group Group { get; set; } = null!;

        public Connection(string connectionId, string userId)
        {
            ConnectionId = connectionId;
            UserId = userId;
        }
        public Connection()
        {

        }
    }
}
