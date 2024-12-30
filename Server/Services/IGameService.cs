using Domain.Models;
using Server.Enums;

namespace Server.Services
{
    public interface IGameService
    {
        void InitializedHands(User user1, User user2);
        void PlayCard(User user1, int index, User user2);
        EndTurnStatus EndTurn(User user1, User user2);
        EndTurnStatus Pass(User user1, User user2);
        void HandleEndTurn(User user1, User user2, EndTurnStatus endTurnStatus);
        void ChangeSign(User user, int index);
        bool IsAutoPass(User user);
    }
}
