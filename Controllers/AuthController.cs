using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtorConnect.Models;
using RealtorConnect.Services.Interfaces;

namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-realtor")]
        public async Task<IActionResult> RegisterRealtor([FromBody] Realtor realtor)
        {
            var success = await _authService.RegisterRealtorAsync(realtor);
            if (!success)
                return BadRequest("Email already exists");

            return Ok(new { Message = "Realtor registered successfully" });
        }

        [HttpPost("register-client")]
        public async Task<IActionResult> RegisterClient([FromBody] Client client)
        {
            var success = await _authService.RegisterClientAsync(client);
            if (!success)
                return BadRequest("Email already exists");

            return Ok(new { Message = "Client registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.LoginAsync(model.Email, model.Password);
            if (result == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { Token = result.Value.Token, Role = result.Value.Role, Id = result.Value.Id });
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}