using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealtorConnect.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> RegisterRealtorAsync(Realtor realtor)
        {
            if (await _context.Realtors.AnyAsync(r => r.Email == realtor.Email))
                return false;

            realtor.Password = BCrypt.Net.BCrypt.HashPassword(realtor.Password);
            _context.Realtors.Add(realtor);

            realtor.GroupId = null; // Не назначаем группу при регистрации

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterClientAsync(Client client)
        {
            if (await _context.Clients.AnyAsync(c => c.Email == client.Email))
                return false;

            client.Password = BCrypt.Net.BCrypt.HashPassword(client.Password);
            _context.Clients.Add(client);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(string Token, string Role, int Id)?> LoginAsync(string email, string password)
        {
            var realtor = await _context.Realtors.FirstOrDefaultAsync(r => r.Email == email);
            if (realtor != null && BCrypt.Net.BCrypt.Verify(password, realtor.Password))
            {
                var token = GenerateJwtToken(realtor.Id.ToString(), "Realtor");
                return (token, "Realtor", realtor.Id);
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
            if (client != null && BCrypt.Net.BCrypt.Verify(password, client.Password))
            {
                var token = GenerateJwtToken(client.Id.ToString(), "Client");
                return (token, "Client", client.Id);
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(c => c.Email == email);
            if (admin != null && BCrypt.Net.BCrypt.Verify(password, admin.Password))
            {
                var token = GenerateJwtToken(admin.Id.ToString(), "Admin");
                return (token, "Admin", admin.Id);
            }

            return null;
        }

        private string GenerateJwtToken(string userId, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}