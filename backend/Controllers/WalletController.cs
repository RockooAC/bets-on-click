using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Wallets;
using WebApi.Services;
using System.Threading.Tasks;
using System;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly WalletService _walletService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<WalletController> _logger;

        public WalletController(
            WalletService walletService,
            IOptions<AppSettings> appSettings,
            ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        // GET: api/wallet/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserWallet(int userId)
        {
            try
            {
                var wallet = await _walletService.GetUserWallet(userId);
                if (wallet == null) return NotFound($"Wallet for user {userId} not found.");

                var walletResponse = new WalletResponse
                {
                    Id = wallet.Id,
                    UserId = wallet.UserId,
                    Amount = wallet.Amount
                };

                return Ok(walletResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting wallet for user {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/wallet/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateWallet(int userId, [FromBody] UpdateWalletRequest request)
        {
            try
            {
                var updatedWallet = await _walletService.UpdateWallet(userId, request.Amount);
                if (updatedWallet == null)
                {
                    return NotFound($"Wallet for user {userId} not found.");
                }

                return Ok(new WalletResponse
                {
                    Id = updatedWallet.Id,
                    UserId = updatedWallet.UserId,
                    Amount = updatedWallet.Amount
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Specific error for invalid amount
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating wallet for user {userId}: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred."); // More specific message
            }
        }

    }
}
