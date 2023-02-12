using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace SignalR_Applications.Hubs
{
    public class NotificationHub : Hub
    {
        public static List<string> Messages { get; set; } = new();

        public async Task SendNotification(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Messages.Add(message);
                await SendAllMessages();
            }
        }

        public async Task SendAllMessages()
        {
            await Clients.All.SendAsync("newMessageLoaded", Messages);
        }

    }
}