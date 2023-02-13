using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR_Applications.Data;

namespace SignalR_Applications.Hubs
{
    public class BasicChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public BasicChatHub(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task SendMessageToAll(string sender, string message)
        {
            await Clients.All.SendAsync("newMessageRecieved", sender, message);
        }

        [Authorize]
        public async Task SendMessageToReciever(string sender, string reciever, string message)
        {
            var recieverId = _context.Users.FirstOrDefault(u => u.Email!.ToLower() == reciever.Trim().ToLower())!.Id;
            if (!string.IsNullOrEmpty(recieverId))
            {
                await Clients.User(recieverId).SendAsync("newMessageRecieved", sender, message);
            }
        }
    }
}