using System;

namespace WebApi.Models.Bets{
    public class BetHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public int EventId { get; set; }
    }
}
