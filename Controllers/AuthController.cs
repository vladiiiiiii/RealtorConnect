using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Добавляем пространство имён
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RealtorConnect.Data;
using RealtorConnect.Models;
using static BCrypt.Net.BCrypt;

namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ...

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (await _context.Clients.AnyAsync(c => c.Email == model.Email) ||
                await _context.Realtors.AnyAsync(r => r.Email == model.Email))
            {
                return BadRequest("Email already exists.");
            }

            if (model.UserType == "Client")
            {
                var client = new Client
                {
                    Name = model.Name,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), 
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }
            else if (model.UserType == "Realtor")
            {
                var realtor = new Realtor
                {
                    Name = model.Name,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), 
                };
                _context.Realtors.Add(realtor);
                await _context.SaveChangesAsync();

                var groupRealtor = new GroupRealtor
                {
                    GroupId = 1,
                    RealtorId = realtor.Id
                };
                _context.GroupRealtors.Add(groupRealtor);
                await _context.SaveChangesAsync();

                realtor.GroupRealtorId = groupRealtor.Id;
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Invalid user type.");
            }

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            Client client = _context.Clients.FirstOrDefault(c => c.Email == model.Email);
            if (client != null && BCrypt.Net.BCrypt.Verify(model.Password, client.PasswordHash))
            {
                var token = GenerateJwtToken(client);
                return Ok(new { Token = token, UserType = "Client" });
            }

            Realtor realtor = _context.Realtors.FirstOrDefault(r => r.Email == model.Email);
            if (realtor != null && BCrypt.Net.BCrypt.Verify(model.Password, realtor.PasswordHash))
            {
                var token = GenerateJwtToken(realtor);
                return Ok(new { Token = token, UserType = "Realtor" });
            }

            return Unauthorized("Invalid credentials.");
        }

        private string GenerateJwtToken(object user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.GetType().Name == "Client" ? ((Client)user).Id.ToString() : ((Realtor)user).Id.ToString()),
                new Claim(ClaimTypes.Role, user.GetType().Name == "Client" ? "Client" : "Realtor")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}