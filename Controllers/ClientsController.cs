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
    [Authorize(Roles = "Realtor")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IRealtorService _realtorService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientService clientService,IRealtorService realtorService,ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _realtorService = realtorService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] Client client)
        {
            try
            {
                var existingClient = await _clientService.GetClientByIdAsync(client.Id);
                if (existingClient != null && existingClient.Email == client.Email)
                {
                    _logger.LogWarning("Attempt to add client with existing email: {Email}", client.Email);
                    return BadRequest("Client with this email already exists");
                }

                client.Password = BCrypt.Net.BCrypt.HashPassword(client.Password);
                client.CreatedByRealtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Устанавливаем риэлтора
                await _clientService.AddClientAsync(client);

                _logger.LogInformation("Client added successfully by Realtor {RealtorId}. ClientId: {ClientId}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value, client.Id);
                return Ok(new { Message = "Client added successfully", ClientId = client.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding client by Realtor {RealtorId}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while adding the client");
            }
        }

        [HttpGet("my-clients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetMyClients(int page = 1, int pageSize = 10)
        {
            try
            {
                var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Получаем всех клиентов через сервис
                var allClients = await _clientService.GetAllClientsAsync();
                var clientsQuery = allClients
                    .Where(c => c.CreatedByRealtorId == realtorId ||
                                c.Apartments.Any(a => a.RealtorId == realtorId))
                    .Distinct();

                var totalClients = clientsQuery.Count();
                var clients = clientsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Realtor {RealtorId} retrieved {ClientCount} clients (page {Page}, pageSize {PageSize})",
                    realtorId, clients.Count, page, pageSize);

                return Ok(new
                {
                    TotalCount = totalClients,
                    Page = page,
                    PageSize = pageSize,
                    Clients = clients
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients for Realtor {RealtorId}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while retrieving clients");
            }
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsForGroup(int groupId, int page = 1, int pageSize = 10)
        {
            try
            {
                var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var realtor = await _realtorService.GetRealtorByIdAsync(realtorId);
                if (realtor == null || realtor.GroupId != groupId)
                    return Forbid("You are not a member of this group");

                var clientsQuery = await _clientService.GetClientsByGroupIdAsync(groupId);

                var totalClients = clientsQuery.Count();
                var clients = clientsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Realtor {RealtorId} retrieved {ClientCount} clients for group {GroupId} (page {Page}, pageSize {PageSize})",
                    realtorId, clients.Count, groupId, page, pageSize);

                return Ok(new
                {
                    TotalCount = totalClients,
                    Page = page,
                    PageSize = pageSize,
                    Clients = clients
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients for group {GroupId} by Realtor {RealtorId}",
                    groupId, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while retrieving clients for the group");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Проверяем, существует ли клиент
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    _logger.LogWarning("Client {ClientId} not found for deletion by Realtor {RealtorId}", id, realtorId);
                    return NotFound("Client not found");
                }

                // Проверяем, создал ли этот риэлтор клиента
                if (client.CreatedByRealtorId != realtorId)
                {
                    _logger.LogWarning("Realtor {RealtorId} attempted to delete client {ClientId} who was not created by them",
                        realtorId, id);
                    return Forbid("You can only delete clients that you created");
                }

                await _clientService.DeleteClientAsync(id);

                _logger.LogInformation("Client {ClientId} deleted successfully by Realtor {RealtorId}", id, realtorId);
                return Ok(new { Message = "Client deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client {ClientId} by Realtor {RealtorId}",
                    id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while deleting the client");
            }
        }
    }
}