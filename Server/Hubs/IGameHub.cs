using Domain.Models;
using Server.Enums;

namespace Server.Hubs
{
    public interface IGameHub
    {
        Task GameFull(string message);
        Task Joined(string roomName);
        Task GameStart(User user1, User user2, int order);
        Task Status(User user1, User user2);
        Task EndRoundStatus(string msg);
        Task EndGame();
    }
}
