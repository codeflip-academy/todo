using System;

namespace TodoWebAPI.Models
{
    public class CardInfoModel
    {
        public string CardType { get; set; }
        public string LastFourDigits { get; set; }
        public string ExpirationDate { get; set; }
    }
}