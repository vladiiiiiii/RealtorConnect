using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealtorConnect.Hubs;
using RealtorConnect.Models;
using RealtorConnect.Models.Dto;
using RealtorConnect.Services.Interfaces;
using System.Security.Claims;

namespace RealtorConnect.Controllers
{
    [Route("api/Clients/{clientId}/Chat")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet("messages/{realtorId}")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatMessages(int clientId, int realtorId)
        {
            try
            {
                // Проверяем, что clientId совпадает с ID авторизованного пользователя
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (clientId != userId)
                {
                    _logger.LogWarning("Client {ClientId} attempted to view messages as Client {RequestedClientId}", userId, clientId);
                    return Unauthorized("You can only view your own messages");
                }

                var messages = await _chatService.GetChatMessagesAsync(clientId, "Client", realtorId, "Realtor");
                _logger.LogInformation("Client {ClientId} retrieved messages with Realtor {RealtorId}", clientId, realtorId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages for Client {ClientId} with Realtor {RealtorId}", clientId, realtorId);
                return StatusCode(500, "An error occurred while retrieving messages");
            }
        }

        [HttpPost("send/{realtorId}")]
        public async Task<IActionResult> SendMessage(int clientId, int realtorId, [FromBody] SendMessageRequest request)
        {
            try
            {
                // Проверяем, что clientId совпадает с ID авторизованного пользователя
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (clientId != userId)
                {
                    _logger.LogWarning("Client {ClientId} attempted to send message as Client {RequestedClientId}", userId, clientId);
                    return Unauthorized("You can only send messages from your own account");
                }

                // Создаем объект ChatMessage
                var message = new ChatMessage
                {
                    SenderId = clientId,
                    SenderType = "Client",
                    ReceiverId = realtorId,
                    ReceiverType = "Realtor",
                    Content = request.Content,
                    SentAt = DateTime.UtcNow // Устанавливаем время отправки на сервере
                };

                // Отправляем сообщение
                await _chatService.SendMessageAsync(message, clientId);

                // Отправляем сообщение через SignalR (только риэлтору)
                await _hubContext.Clients.User(realtorId.ToString()).SendAsync("ReceiveMessage", message);

                _logger.LogInformation("Client {ClientId} sent message to Realtor {RealtorId}", clientId, realtorId);
                return Ok(new { Message = "Message sent successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message from Client {ClientId} to Realtor {RealtorId}", clientId, realtorId);
                return StatusCode(500, "An error occurred while sending the message");
            }
        }
    }
}