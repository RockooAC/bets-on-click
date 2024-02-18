using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BetHistoryController : ControllerBase
    {
        private readonly IBetHistoryService _betHistoryService;

        public BetHistoryController(IBetHistoryService betHistoryService)
        {
            _betHistoryService = betHistoryService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetBetHistory(int userId)
        {
            var betHistory = _betHistoryService.GetBetHistory(userId);
            return Ok(betHistory);
        }
    }
}