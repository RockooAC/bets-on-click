using System;

namespace WebApi.Entities
{
    public class Bet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public int EventId { get; set; }

        // Relacje
        public User User { get; set; }
        public Event Event { get; set; }
    }
}
