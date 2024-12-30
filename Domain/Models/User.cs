namespace Domain.Models
{
    public class User
    {
        public string Id { get; set; }
        public int Points { get; set; }
        public int Count
        {
            get => Board.Sum(e => e.Number);
        }
        public string Name { get; set; }
        public bool IsTurn { get; set; }
        public bool IsCardPlayed { get; set; }
        public bool IsPassed { get; set; }
        public List<Card> Hand { get; set; } = new();
        public List<Card> Board { get; set; } = new();
    }
}
