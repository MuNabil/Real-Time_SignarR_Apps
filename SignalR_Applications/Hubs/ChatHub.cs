using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using SignalR_Applications.Data;

namespace SignalR_Applications.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public ChatHub(ApplicationDbContext context)
        {
            _context = context;

        }
        public override Task OnConnectedAsync()
        {
            var UserId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!String.IsNullOrEmpty(UserId))
            {
                var userName = _context.Users.FirstOrDefault(u => u.Id == UserId)!.UserName;
                // To send that a user loged in to all online users
                Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserConnected", userName);
                HubConnections.AddUserConnection(UserId, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var UserId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);

            if (HubConnections.HasUserConnection(UserId, Context.ConnectionId))
            {
                var UserConnections = HubConnections.Users[UserId];
                UserConnections.Remove(Context.ConnectionId);

                HubConnections.Users.Remove(UserId);
                if (UserConnections.Any())
                    HubConnections.Users.Add(UserId, UserConnections);
            }

            if (!String.IsNullOrEmpty(UserId))
            {
                var userName = _context.Users.FirstOrDefault(u => u.Id == UserId)!.UserName;
                Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserDisconnected", userName);
                HubConnections.AddUserConnection(UserId, Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        // Client will incoke it after adding a new room with ajax then it will notify all this hub clients
        public async Task SendAddRoomMessage(int maxRoom, int roomId, string roomName)
        {
            var UserId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _context.Users.FirstOrDefault(u => u.Id == UserId)!.UserName;

            await Clients.All.SendAsync("ReceiveAddRoomMessage", maxRoom, roomId, roomName, UserId, userName);
        }

        public async Task SendDeleteRoomMessage(int deleted, int selected, string roomName)
        {
            var UserId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _context.Users.FirstOrDefault(u => u.Id == UserId)!.UserName;

            await Clients.All.SendAsync("ReceiveDeleteRoomMessage", deleted, selected, roomName, userName);
        }

        public async Task SendPublicMessage(int roomId, string message, string roomName)
        {
            var UserId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _context.Users.FirstOrDefault(u => u.Id == UserId)!.UserName;

            await Clients.All.SendAsync("ReceivePublicMessage", roomId, UserId, userName, message, roomName);
        }

        public async Task SendPrivateMessage(string receiverId, string message, string receiverName)
        {
            var senderId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var senderName = _context.Users.FirstOrDefault(u => u.Id == senderId)!.UserName;

            var users = new string[] { senderId, receiverId };

            await Clients.Users(users).SendAsync("ReceivePrivateMessage", senderId, senderName, receiverId, message, Guid.NewGuid(), receiverName);
        }
    }
}