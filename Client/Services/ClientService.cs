using Domain.Models;

namespace Client.Services
{
    public class ClientService : IClientService
    {

        public bool IsConnected { get; set ; }
        public bool IsStarted { get; set; }
        public string RoomName { get; set; } = "";
        public string Message { get; set; } = "";
        public int Order { get; set; }
        public List<string> Messages { get; set; } = new();
        public User User1 { get; set; } = new();
        public User User2 { get; set; } = new();

        public void EndGame()
        {
            IsConnected = false;
            IsStarted = false;
            Messages.Add($"Выход из команты {RoomName}");
        }

        public void EndRoundStatus(string msg)
        {
            Message = msg;
        }

        public void GameFull(string msg)
        {
            Messages.Add(msg);
        }

        public void GameStart(User user1, User user2, int order)
        {
            User1 = user1;
            User2 = user2;
            Order = order;
            IsStarted = true;
        }

        public void Joined(string roomName)
        {
            Messages.Add($"Вход в комнату {roomName}");
            RoomName = roomName;
            IsConnected = true;
        }

        public void ReceiveMessage(string msg)
        {
            Messages.Add(msg);
        }

        public void Status(User user1, User user2)
        {
            User1 = user1;
            User2 = user2;
            Message = "";
        }
    }
}
