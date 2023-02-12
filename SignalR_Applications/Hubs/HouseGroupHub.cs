using Microsoft.AspNetCore.SignalR;

namespace SignalR_Applications.Hubs
{
    public class HouseGroupHub : Hub
    {
        // To track all conections that are subscribed to a specific house
        public static List<string> GroupsJoined { get; set; } = new List<string>();

        public async Task JoinHouse(string houseName)
        {
            if (!GroupsJoined.Contains(Context.ConnectionId + ":" + houseName))
            {
                GroupsJoined.Add(Context.ConnectionId + ":" + houseName);

                await GetHouses(houseName.ToLower(), true);

                // Notify all other users that subscribe to this house that there is a new subscribe user
                await Clients.OthersInGroup(houseName).SendAsync("newMemberJoinedToHouse", houseName);

                await Groups.AddToGroupAsync(Context.ConnectionId, houseName);
            }
        }

        public async Task LeaveHouse(string houseName)
        {
            if (GroupsJoined.Contains(Context.ConnectionId + ":" + houseName))
            {
                GroupsJoined.Remove(Context.ConnectionId + ":" + houseName);

                await GetHouses(houseName.ToLower(), false);

                await Clients.OthersInGroup(houseName).SendAsync("newMemberLeaveFromHouse", houseName);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, houseName);
            }
        }

        public async Task TriggerHouseNotify(string houseName)
        {
            await Clients.Group(houseName).SendAsync("houseNotificationTriggered", houseName);
        }

        private async Task GetHouses(string houseName, bool hasSubscribed)
        {
            // To get the list of houses that user subscribes to
            string houseList = "";
            GroupsJoined.Where(x => x.Contains(Context.ConnectionId))
                .Select(x => houseList += x.Split(':')[1] + "  ").ToList();

            // To send these list of houses to the caller to update his subscribtion List
            await Clients.Caller.SendAsync("subscriptionStatus", houseList, houseName, hasSubscribed);


            // Imparative way
            // string houseList = "";
            // foreach (var str in GroupsJoined)
            // {
            //     if (str.Contains(Context.ConnectionId))
            //     {
            //         houseList += str.Split(':')[1] + " ";
            //     }
            // }
        }
    }
}
