using Domain.Models;
using Server.Enums;
using System;

namespace Server.Services
{
    public class GameService : IGameService
    {
        private readonly Random random = new();

        private Card GenerateRandomCard()
        {
            var card = new Card();
            if (random.Next(0, 2) == 1)
            {
                card.Type = CardType.Default;
                card.Number = Convert.ToBoolean(random.Next(0, 2))
                            ? random.Next(-6, 0)
                            : random.Next(1, 7);
            }
            else
            {
                card.Type = CardType.Inverse;
                card.Number = random.Next(1, 7);
            }
            return card;
        }

        public void InitializedHands(User user1, User user2)
        {
            for (int i = 0; i < 4; i++)
            {
                user1.Hand.Add(GenerateRandomCard());
                user2.Hand.Add(GenerateRandomCard());
            }

            PlayDefault(user1);
            user1.IsTurn = true;
        }

        public void PlayCard(User user1, int index, User user2)
        {
            if (user1.IsTurn && !user1.IsCardPlayed) 
            {
                var card = user1.Hand[index];
                user1.Hand.RemoveAt(index);
                user1.Board.Add(card);
                user1.IsCardPlayed = true;
            }
        }

        private void PlayDefault(User user1)
        {
            user1.Board.Add(new Card
            {
                Number = random.Next(1, 11),
                Type = CardType.NotUser
            });
        }

        public void ChangeSign(User user, int index)
        {
            user.Hand[index].InverseValue();
        }

        public bool IsAutoPass(User user1)
        {
            return user1.Count == 20 || user1.Board.Count == 9;
        }

        public EndTurnStatus EndTurn(User user1, User user2)
        {
            if (user1.IsTurn)
            { 
                if (user1.Count > 20)
                {
                    user2.Points++;
                    if (user2.Points == 3)
                        return EndTurnStatus.User2WonGame;
                    return EndTurnStatus.User2WonRound;
                }

                if (user2.IsPassed)
                {
                    if (user1.IsPassed)
                    {
                        if (user1.Count > user2.Count)
                        {
                            user1.Points++;
                            if (user1.Points == 3)
                                return EndTurnStatus.User1WonGame;
                            return EndTurnStatus.User1WonRound;
                        }
                        else if (user2.Count > user1.Count)
                        {
                            user2.Points++;
                            if (user2.Points == 3)
                                return EndTurnStatus.User2WonGame;
                            return EndTurnStatus.User2WonRound;
                        }
                        else 
                            return EndTurnStatus.DrawRound;
                    }
                    return EndTurnStatus.User2EndTurn;
                }
                return EndTurnStatus.User1EndTurn;
            }

            return EndTurnStatus.None;
        }

        public void HandleEndTurn(User user1, User user2, EndTurnStatus endTurnStatus)
        {
            switch (endTurnStatus)
            {
                case EndTurnStatus.User1EndTurn:
                    {
                        user1.IsTurn = false;
                        user1.IsCardPlayed = false;
                        user2.IsTurn = true;
                        PlayDefault(user2);
                        return;
                    }
                case EndTurnStatus.User2EndTurn:
                    {
                        user2.IsTurn = false;
                        user2.IsCardPlayed = false;
                        user1.IsTurn = true;
                        PlayDefault(user1);
                        return;
                    }
                case EndTurnStatus.User1WonRound:
                    {
                        user1.Board.Clear();
                        user2.Board.Clear();
                        user1.IsPassed = false;
                        user2.IsPassed = false;

                        user1.IsTurn = false;
                        user1.IsCardPlayed = false;
                        user2.IsTurn = true;
                        PlayDefault(user2);
                        return;
                    }
                case EndTurnStatus.User2WonRound:
                    {
                        user1.Board.Clear();
                        user2.Board.Clear();
                        user1.IsPassed = false;
                        user2.IsPassed = false;

                        user2.IsTurn = false;
                        user2.IsCardPlayed = false;
                        user1.IsTurn = true;
                        PlayDefault(user1);
                        return;
                    }
                case EndTurnStatus.DrawRound:
                    {
                        user1.Board.Clear();
                        user2.Board.Clear();
                        user1.IsPassed = false;
                        user2.IsPassed = false;

                        user2.IsTurn = false;
                        user2.IsCardPlayed = false;
                        user1.IsTurn = true;
                        PlayDefault(user1);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public EndTurnStatus Pass(User user1, User user2)
        {
            if (user1.IsTurn)
            {
                user1.IsPassed = true;
                return EndTurn(user1, user2);
            }
            return EndTurnStatus.None;
        }

    }
}
