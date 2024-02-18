using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Bets
{
    public class PlaceBetRequest
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int EventId { get; set; }

    }
}
