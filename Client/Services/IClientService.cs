using Domain.Models;

namespace Client.Services
{
    public interface IClientService
    {
        bool IsConnected { get; set; }
        bool IsStarted { get; set; }
        string RoomName { get; set; }
        string Message { get; set; }
        int Order { get; set; }
        List<string> Messages { get; set; }
        User User1 { get; set; }
        User User2 { get; set; }

        void Joined(string roomName);
        void ReceiveMessage(string msg);
        void GameFull(string msg);
        void EndRoundStatus(string msg);
        void GameStart(User user1, User user2, int order);
        void Status(User user1, User user2);
        void EndGame();
    }
}
