namespace Domain.Models
{
    public class Card
    {
        public int Number { get; set; }
        public CardType Type { get; set; }
        public void InverseValue()
        {
            if (Type == CardType.Inverse)
            {
                Number = -Number;
            }
        }
        public string ImageUrl 
        {
            get 
            {
                if (Type == CardType.NotUser)
                    return "cards/green_card.png";
                else if (Type == CardType.Default)
                    return (Number > 0) ? "cards/blue_card.png" : "cards/red_card.png";
                else if (Type == CardType.Inverse)
                    return (Number > 0) ? "cards/blue_red_card.png" : "cards/red_blue_card.png";
                return "card/card_back.png";
            }
        }
    }
}
