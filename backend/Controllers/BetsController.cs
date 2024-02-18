using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Bets;
using WebApi.Services;
using System;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users/{userId}/bets")]
    public class BetsController : ControllerBase
    {
        private readonly IBetService _betService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger<BetsController> _logger;

        public BetsController(
            IBetService betService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ILogger<BetsController> logger)
        {
            _betService = betService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        // POST: users/{userId}/bets/place
        [AllowAnonymous]
        [HttpPost("place")]
        public IActionResult PlaceBet(int userId, [FromBody] PlaceBetRequest model)
        {

            _betService.PlaceBet(userId, model.EventId, model.Amount);

            return Ok(new { message = "Successful" });
        }

        //GET: users/{userId}/bets
        [HttpGet]
        public IActionResult GetAllBets(int userId)
        {
            var bets = _betService.GetAllBets(userId);
            return Ok(bets);
        }

        // GET: users/{userId}/bets/{id}
        [HttpGet("{id}")]
        public IActionResult GetBetById(int userId, int id)
        {
            var bet = _betService.GetBetById(userId, id);
            return Ok(bet);
        }

        // DELETE: users/{userId}/bets/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteBet(int userId, int id)
        {
            _betService.DeleteBet(userId, id);
            return Ok(new { message = "Bet deleted successfully" });
        }
}
}
