using DatingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Domain.RepositoryContracts
{
    public interface IGroupRepository
    {
        void AddGroup(Group group);
        Task RemoveConnection(string connectionId);
        Task<Connection?> GetConnection(string connectionId);
        Task<Group?> GetMessageGroup(string groupName);
        Task<Group?> GetGroupForConnection(string connectionId);
        public Task<bool> SaveAllChangesAsync();

    }
}
