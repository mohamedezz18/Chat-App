using DatingApp.Core.Domain.Entities;
using DatingApp.Core.DTO;
using DatingApp.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace DatingApp.UI.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private IMessageService _messageService;
        private IGroupService _groupService;
        private IHubContext<PresenceHub> _PresenceHub; 
        public MessageHub(IMessageService messageService, IGroupService groupService, IHubContext<PresenceHub> presenceHub)
        {
            _messageService = messageService;
            _groupService = groupService;
            _PresenceHub = presenceHub;
        }


        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request?.Query["userId"].ToString()
                ?? throw new HubException("Other user not found");
            var groupName = GetGroupName(GetUserId().ToString(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await _groupService.AddConnectionToGroupAsync(groupName,
                 Context.ConnectionId, GetUserId().ToString());

            var messages = await _messageService.GetMessageThreadAsync(GetUserId(), Guid.Parse(otherUser));

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        private static string GetGroupName(string? caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var SenderID = GetUserId();
            if (SenderID == createMessageDto.RecipientId)
            {
                throw new HubException("Cannot Send Message To Yourself");
            }
            string groupName = GetGroupName(SenderID.ToString(), createMessageDto.RecipientId.ToString());
            var group =await _groupService.GetMessageGroupAsync(groupName);
            bool userInGroup = group != null && group.Connections.Any(x =>
            x.UserId == createMessageDto.RecipientId.ToString());

            var messageDto = userInGroup? await _messageService.CreateMessageAndReadAsync(createMessageDto, SenderID) 
                : await _messageService.CreateMessageAsync(createMessageDto, SenderID);

            if (messageDto != null)
            {
                await Clients.Group(groupName).SendAsync("NewMessage", messageDto);
                var connections = await PresenceTracker.GetConnectionsForUser(messageDto.RecipientId.ToString());
                if (connections != null && connections.Count > 0 && !userInGroup)
                {
                    await _PresenceHub.Clients.Clients(connections)
                   .SendAsync("NewMessageReceived", messageDto);
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _groupService.RemoveConnectionFromGroupAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        private Guid GetUserId()
        {
            return Context.User?.GetMemberId()
                ?? throw new HubException("Cannot get member id");
        }
    }
}
