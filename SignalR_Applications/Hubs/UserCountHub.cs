using Microsoft.AspNetCore.SignalR;

namespace SignalR_Applications.Hubs
{
    public class UserCountHub : Hub
    {
        public static int TotalViews { get; set; } = 0;
        public static int TotalUsers { get; set; } = 0;

        public override Task OnConnectedAsync()
        {
            TotalUsers++;
            Clients.All.SendAsync("updateTotalUsers", TotalUsers).GetAwaiter().GetResult();
            // .GetAwaiter().GetResult() => because i can not use await here.
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? ex)
        {
            TotalUsers--;
            Clients.All.SendAsync("updateTotalUsers", TotalUsers).GetAwaiter().GetResult();
            return base.OnDisconnectedAsync(ex);
        }

        public async Task NewWindowLoaded()
        {
            TotalViews++;

            // Send update to all clients connecting to this hub that views have been updated
            await Clients.All.SendAsync("UpdateTotalViews", TotalViews);
        }
    }
}
