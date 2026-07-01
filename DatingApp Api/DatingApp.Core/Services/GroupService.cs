using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Services
{
    public class GroupService : IGroupService
    {

        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<bool> AddConnectionToGroupAsync(string groupName,
            string connectionId, string userId)
        {
            var group = await _groupRepository.GetMessageGroup(groupName);

            if (group == null)
            {
                group = new Group(groupName);
                _groupRepository.AddGroup(group);
            }

            var existingConnection = await _groupRepository.GetConnection(connectionId);
            if (existingConnection == null)
            {
                group.Connections.Add(new Connection(connectionId, userId));
            }

            return await _groupRepository.SaveAllChangesAsync();

        }

        public async Task<bool> RemoveConnectionFromGroupAsync(string connectionId)
        {
            var connection = await _groupRepository.GetConnection(connectionId);

            if (connection == null)
            {
                return false;
            }

            await _groupRepository.RemoveConnection(connectionId);
            return await _groupRepository.SaveAllChangesAsync();
        }

        public async Task<Connection?> GetConnectionAsync(string connectionId)
        {
            return await _groupRepository.GetConnection(connectionId);
        }

        public async Task<Group?> GetMessageGroupAsync(string groupName)
        {
            return await _groupRepository.GetMessageGroup(groupName);
        }

        public async Task<Group?> GetGroupForConnectionAsync(string connectionId)
        {
            return await _groupRepository.GetGroupForConnection(connectionId);
        }
    }
}
