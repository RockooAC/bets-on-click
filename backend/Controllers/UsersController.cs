using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Models.Wallets;
using WebApi.Services;
using System.Threading.Tasks;
using System;


namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly WalletService _walletService;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            WalletService walletService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _walletService = walletService;
        }

        // POST: /users/authenticate
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return Ok(response);
        }

        // POST: /users/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            _userService.Register(model);
            var userId = _userService.GetLastRegisteredUserId(model.id);
            // Create a wallet for the user
            await _walletService.CreateWalletAsync(userId);
            return Ok(new { message = "Registration successful" });
        }

        // GET: /users
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        // GET: /users/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            return Ok(user);
        }

        // PUT: /users/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRequest model)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Role = model.Role;

            try
            {
                _userService.Update(id, model);
                return Ok(new { message = "User updated successfully" });
            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating the user" });
            }
        }




        // DELETE: /users/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok(new { message = "User deleted successfully" });
        }
    }
}
