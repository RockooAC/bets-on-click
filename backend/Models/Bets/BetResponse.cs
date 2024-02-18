using System;

namespace WebApi.Models.Bets
{
    public class BetResponse
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int EventId { get; set; }
    }
}
