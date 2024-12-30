using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, List<string>> chatRooms = new();

        public async Task JoinChat(string roomName)
        {
            if (!chatRooms.ContainsKey(roomName))
                chatRooms[roomName] = new List<string>();

            if (chatRooms[roomName].Count >= 2)
            {
                await Clients.Caller.SendAsync("ChatFull", "The chat room is full.");
                return;
            }

            chatRooms[roomName].Add(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Caller.SendAsync("Joined", roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the chat.");
        }

        public async Task SendMessage(string roomName, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var room in chatRooms.Keys)
            {
                chatRooms[room].Remove(Context.ConnectionId);
                await Clients.Group(room).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left the chat.");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}