using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DatingApp.UI.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private PresenceTracker _presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }
        public override async Task OnConnectedAsync()
        {
            await _presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserOnline", GetUserId());

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _presenceTracker.UserDisconnected(GetUserId(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserOffline", GetUserId());

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserId()
        {
            return Context.User?.GetMemberId().ToString()
                ?? throw new HubException("Cannot get member id");
        }
    }
}
