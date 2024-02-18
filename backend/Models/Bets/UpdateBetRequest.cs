using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Bets
{
    public class UpdateBetRequest
    {
        [Required]
        public decimal Amount { get; set; }

        public int EventId { get; set; }
    }
}
