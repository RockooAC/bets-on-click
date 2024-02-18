using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using Microsoft.Extensions.Logging;
using System;

namespace WebApi.Services
{
    public class WalletService
    {
        private readonly DataContext _context;
        private readonly ILogger<WalletService> _logger;

        public WalletService(DataContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Wallet> GetUserWallet(int userId)
        {
            var wallet = await _context.Wallets
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(w => w.UserId == userId);

            // Create wallet if it doesn't exist
            if (wallet == null)
            {
                wallet = await CreateWalletAsync(userId);
            }

            return wallet;
        }

        public async Task<bool> Deposit(int userId, decimal amount)
        {
            try
            {
                var wallet = await GetUserWallet(userId);
                if (wallet == null)
                {
                    return false;
                }

                wallet.Amount += amount;
                _context.Entry(wallet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error depositing to wallet for user {userId}: {ex.Message}");
                throw; // Re-throw the exception for proper handling
            }
        }

        public async Task<Wallet> Withdraw(int userId, decimal amount)
        {
            try
            {
                var wallet = await GetUserWallet(userId);
                if (wallet == null || wallet.Amount < amount)
                {
                    return null;
                }

                wallet.Amount -= amount;
                _context.Entry(wallet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return wallet;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error withdrawing from wallet for user {userId}: {ex.Message}");
                throw; // Re-throw the exception for proper handling
            }
        }

        public async Task<Wallet> UpdateWallet(int userId, decimal amount)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("Invalid amount: negative value provided.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var wallet = await GetUserWallet(userId);
                    if (wallet == null)
                    {
                        return null; // Wallet doesn't exist, nothing to update
                    }

                    wallet.Amount += amount;
                    _context.Entry(wallet).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return wallet;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"Error updating wallet for user {userId}: {ex.Message}");
                    throw; // Re-throw for proper handling
                }
            }
        }

        public async Task<Wallet> CreateWalletAsync(int userId)
        {
            var wallet = new Wallet { UserId = userId, Amount = 0 };
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }
    }
}
