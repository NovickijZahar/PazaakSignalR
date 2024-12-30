using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Domain.Models;
using Server.Services;
using Server.Enums;

namespace Server.Hubs
{
    public class GameHub: Hub<IGameHub>
    {
        private readonly static ConcurrentDictionary<string, List<User>> gameRooms = new();
        private IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task Join(string roomName, string userName)
        {   
            if (!gameRooms.ContainsKey(roomName))
                gameRooms[roomName] = new List<User>();

            if (gameRooms[roomName].Count >= 2)
            {
                await Clients.Caller.GameFull("The game room is full.");
                return;
            }

            gameRooms[roomName].Add(new User { Name=userName, Id=Context.ConnectionId});
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Caller.Joined(roomName);

            if (gameRooms[roomName].Count == 2)
            {
                var user1 = gameRooms[roomName][0];
                var user2 = gameRooms[roomName][1];

                _gameService.InitializedHands(user1, user2);

                await Clients.Client(user1.Id).GameStart(user1, user2, 0);
                await Clients.Client(user2.Id).GameStart(user2, user1, 1);
            }
        }

        public async Task PlayCard(string roomName, int order, int index)
        {
            var user1 = gameRooms[roomName][order];
            var user2 = gameRooms[roomName][1 - order];

            _gameService.PlayCard(user1, index, user2);
            if (_gameService.IsAutoPass(user1))
            {
                await Pass(roomName, order);
                return;
            }

            await Clients.Client(user1.Id).Status(user1, user2);
            await Clients.Client(user2.Id).Status(user2, user1);
        }

        public async Task EndTurn(string roomName, int order)
        {
            var user1 = gameRooms[roomName][order];
            var user2 = gameRooms[roomName][1 - order];

            var endTurnStatus = _gameService.EndTurn(user1, user2);
            await Clients.Client(user1.Id).Status(user1, user2);
            await Clients.Client(user2.Id).Status(user2, user1);

            var response = await HandleEndTurn(user1, user2, endTurnStatus);
            if (response)
            {
                if (_gameService.IsAutoPass(user2))
                {
                    await Pass(roomName, 1 - order);
                    return;
                }
                await Clients.Client(user1.Id).Status(user1, user2);
                await Clients.Client(user2.Id).Status(user2, user1);
            }
        }

        public async Task Pass(string roomName, int order)
        {
            var user1 = gameRooms[roomName][order];
            var user2 = gameRooms[roomName][1 - order];

            var endTurnStatus = _gameService.Pass(user1, user2);
            await Clients.Client(user1.Id).Status(user1, user2);
            await Clients.Client(user2.Id).Status(user2, user1);

            var response = await HandleEndTurn(user1, user2, endTurnStatus);
            if (response)
            {
                if (_gameService.IsAutoPass(user2))
                {
                    await Pass(roomName, 1 - order);
                    return;
                }
                await Clients.Client(user1.Id).Status(user1, user2);
                await Clients.Client(user2.Id).Status(user2, user1);
            }
        }

        public async Task ChangeSign(string roomName, int order, int index)
        {
            var user1 = gameRooms[roomName][order];
            var user2 = gameRooms[roomName][1 - order];

            _gameService.ChangeSign(user1, index);

            await Clients.Client(user1.Id).Status(user1, user2);
            await Clients.Client(user2.Id).Status(user2, user1);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await EndGame();
            await base.OnDisconnectedAsync(ex);
        }

        private async Task EndGame()
        {
            var roomKey = gameRooms
                .Where(room => room.Value.Any(u => u.Id == Context.ConnectionId))
                .Select(room => room.Key)
                .FirstOrDefault();

            var count = gameRooms[roomKey].Count;

            if (roomKey != null)
            {
                await Clients.Group(roomKey).EndGame();
                await Groups.RemoveFromGroupAsync(gameRooms[roomKey][0].Id, roomKey);
                if (count == 2)
                {
                    await Groups.RemoveFromGroupAsync(gameRooms[roomKey][1].Id, roomKey);
                }
                gameRooms.TryRemove(roomKey, out _);
                var groups = Groups;
            }
        }

        private async Task<bool> HandleEndTurn(User user1, User user2, EndTurnStatus endTurnStatus)
        {
            if (endTurnStatus == EndTurnStatus.None)
                return false;
            else if (endTurnStatus == EndTurnStatus.User1WonRound)
            {
                await Clients.Client(user1.Id).EndRoundStatus("Вы выиграли раунд");
                await Clients.Client(user2.Id).EndRoundStatus("Вы проиграли раунд");
                await Task.Delay(3000);
            }
            else if (endTurnStatus == EndTurnStatus.User2WonRound)
            {
                await Clients.Client(user2.Id).EndRoundStatus("Вы выиграли раунд");
                await Clients.Client(user1.Id).EndRoundStatus("Вы проиграли раунд");
                await Task.Delay(3000);
            }
            else if (endTurnStatus == EndTurnStatus.DrawRound)
            {
                await Clients.Client(user2.Id).EndRoundStatus("Ничья");
                await Clients.Client(user1.Id).EndRoundStatus("Ничья");
                await Task.Delay(3000);
            }
            else if (endTurnStatus == EndTurnStatus.User1WonGame)
            {
                await Clients.Client(user1.Id).EndRoundStatus("Вы победили");
                await Clients.Client(user2.Id).EndRoundStatus("Вы проиграли");
                await Task.Delay(3000);
                await EndGame();
                return false;
            }
            else if (endTurnStatus == EndTurnStatus.User2WonGame)
            {
                await Clients.Client(user2.Id).EndRoundStatus("Вы победили");
                await Clients.Client(user1.Id).EndRoundStatus("Вы проиграли");
                await Task.Delay(3000);
                await EndGame();
                return false;
            }

            _gameService.HandleEndTurn(user1, user2, endTurnStatus);
            return true;
        }
    }
}
