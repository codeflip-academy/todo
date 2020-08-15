using System;

namespace Todo.Infrastructure
{
    public class CardInfoDto
    {
        public string CardType { get; set; }
        public string LastFourDigits { get; set; }
        public string ExpirationDate { get; set; }
    }
}