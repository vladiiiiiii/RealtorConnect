using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealtorConnect.Models;
using RealtorConnect.Services.Interfaces;
using System.Security.Claims;

namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class ClientProfileController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientProfileController> _logger;

        public ClientProfileController(IClientService clientService, ILogger<ClientProfileController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        // Получение данных текущего клиента
        [HttpGet("me")]
        public async Task<ActionResult<Client>> GetClientProfile()
        {
            try
            {
                var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var client = await _clientService.GetClientByIdAsync(clientId);

                if (client == null)
                {
                    _logger.LogWarning("Client {ClientId} not found", clientId);
                    return NotFound("Client not found");
                }

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile for Client {ClientId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while retrieving the client profile");
            }
        }

        // Обновление данных текущего клиента
        [HttpPut("me")]
        public async Task<IActionResult> UpdateClientProfile([FromBody] Client updatedClient)
        {
            try
            {
                var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Проверяем, что клиент обновляет только свой профиль
                if (updatedClient.Id != clientId)
                {
                    _logger.LogWarning("Client {ClientId} attempted to update another client's profile {RequestedClientId}", clientId, updatedClient.Id);
                    return Forbid("You can only update your own profile");
                }

                var existingClient = await _clientService.GetClientByIdAsync(clientId);
                if (existingClient == null)
                {
                    _logger.LogWarning("Client {ClientId} not found for update", clientId);
                    return NotFound("Client not found");
                }

                // Проверяем, не используется ли новый email другим клиентом
                if (existingClient.Email != updatedClient.Email)
                {
                    var emailCheck = await _clientService.GetAllClientsAsync();
                    if (emailCheck.Any(c => c.Id != clientId && c.Email == updatedClient.Email))
                    {
                        _logger.LogWarning("Client {ClientId} attempted to use an existing email: {Email}", clientId, updatedClient.Email);
                        return BadRequest("This email is already in use");
                    }
                }

                // Обновляем данные клиента
                existingClient.FirstName = updatedClient.FirstName;
                existingClient.LastName = updatedClient.LastName;
                existingClient.Email = updatedClient.Email;
                existingClient.Phone = updatedClient.Phone;
                // Пароль можно обновить отдельно через другой эндпоинт, если нужно
                if (!string.IsNullOrEmpty(updatedClient.Password))
                {
                    existingClient.Password = BCrypt.Net.BCrypt.HashPassword(updatedClient.Password);
                }

                await _clientService.UpdateClientAsync(existingClient);

                _logger.LogInformation("Client {ClientId} updated their profile successfully", clientId);
                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for Client {ClientId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while updating the client profile");
            }
        }
    }
}