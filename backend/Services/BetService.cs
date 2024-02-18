using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Bets;
using System;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services
{
    public interface IBetService
    {
        Bet PlaceBet(int userId, int eventId, decimal amount);
        IEnumerable<Bet> GetAllBets(int userId);
        Bet GetBetById(int userId, int betId);
        void UpdateBet(int userId, int betId, UpdateBetRequest model);
        void DeleteBet(int userId, int betId);
    }

    public class BetService : IBetService
    {
        private DataContext _context;
        private readonly IMapper _mapper;

        public BetService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Bet PlaceBet(int userId, int eventId, decimal amount)
        {
            // Tworzenie nowego obiektu Bet z podanymi danymi
            var bet = new Bet
            {
                UserId = userId,
                EventId = eventId,
                Amount = amount
            };

            _context.Bets.Add(bet);
            _context.SaveChanges();
            return bet;
        }


        public IEnumerable<Bet> GetAllBets(int userId)
        {
            return _context.Bets
                .Where(b => b.UserId == userId)
                .Include(b => b.User)
                .Include(b => b.Event)
                .ToList();
        }

        public Bet GetBetById(int userId, int id)
        {
            return _context.Bets
                .Include(b => b.User)
                .Include(b => b.Event)
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);
        }

        public void UpdateBet(int userId, int betId, UpdateBetRequest model)
        {
            var bet = _context.Bets.Find(betId);

            if (bet == null || bet.UserId != userId)
            {
                throw new KeyNotFoundException("Bet not found or user not authorized");
            }

            _mapper.Map(model, bet);

            _context.Bets.Update(bet);
            _context.SaveChanges();
        }

        public void DeleteBet(int userId, int betId)
        {
            var bet = _context.Bets.Find(betId);

            if (bet == null || bet.UserId != userId)
            {
                throw new KeyNotFoundException("Bet not found or user not authorized");
            }

            _context.Bets.Remove(bet);
            _context.SaveChanges();
        }
    }
}
