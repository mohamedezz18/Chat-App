using DatingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.ServiceContracts
{
    public interface IGroupService
    {

        Task<bool> AddConnectionToGroupAsync(string groupName, string connectionId, string userId);
        Task<bool> RemoveConnectionFromGroupAsync(string connectionId);
        Task<Connection?> GetConnectionAsync(string connectionId);
        Task<Group?> GetMessageGroupAsync(string groupName);
        Task<Group?> GetGroupForConnectionAsync(string connectionId);
    }
}
