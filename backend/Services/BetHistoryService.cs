using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Bets;

namespace WebApi.Services{
    public interface IBetHistoryService
    {
        IEnumerable<BetHistory> GetBetHistory(int userId);
    }

    public class BetHistoryService : IBetHistoryService
    {
        private readonly DataContext _context;

        public BetHistoryService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<BetHistory> GetBetHistory(int userId)
        {
            return _context.Set<BetHistory>().Where(b => b.UserId == userId).ToList();
        }
    }
}