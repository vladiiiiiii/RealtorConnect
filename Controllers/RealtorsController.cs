using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealtorConnect.Hubs;
using RealtorConnect.Models;
using RealtorConnect.Models.Dto;
using RealtorConnect.Services;
using RealtorConnect.Services.Interfaces;
using System.Security.Claims;


namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealtorsController : ControllerBase
    {
        private readonly IRealtorService _realtorService;
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<RealtorsController> _logger;

        public RealtorsController(IRealtorService realtorService, IChatService chatService, ILogger<RealtorsController> logger, IHubContext<ChatHub> hubContext)
        {
            _realtorService = realtorService;
            _chatService = chatService;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Realtor>>> GetRealtors()
        {
            return await _realtorService.GetAllRealtorsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Realtor>> GetRealtor(int id)
        {
            var realtor = await _realtorService.GetRealtorByIdAsync(id);
            if (realtor == null) return NotFound();
            return realtor;
        }

        [HttpPost]
        public async Task<ActionResult<Realtor>> PostRealtor(Realtor realtor)
        {
            await _realtorService.AddRealtorAsync(realtor);
            return CreatedAtAction(nameof(GetRealtor), new { id = realtor.Id }, realtor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> PutRealtor(int id, Realtor realtor)
        {
            try
            {
                if (id != realtor.Id)
                    return BadRequest("Realtor ID mismatch");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != id)
                {
                    _logger.LogWarning("Realtor {UserId} attempted to update Realtor {RealtorId}", userId, id);
                    return Forbid("You can only update your own profile");
                }

                await _realtorService.UpdateRealtorAsync(realtor);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Realtor {RealtorId} by Realtor {UserId}", id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRealtor(int id)
        {
            try
            {
                await _realtorService.DeleteRealtorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Realtor {RealtorId} by Admin", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/upload-photo")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != id)
                {
                    _logger.LogWarning("Realtor {UserId} attempted to upload photo for Realtor {RealtorId}", userId, id);
                    return Forbid("You can only upload photos for your own profile");
                }

                var photoUrl = await _realtorService.UploadPhotoAsync(id, file);
                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading photo for Realtor {RealtorId} by Realtor {UserId}", id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{realtorId}/chat/{clientId}")]
        [Authorize(Roles = "Realtor,Client")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatMessages(int realtorId, int clientId)
        {
            try
            {
                // Получаем ID и роль авторизованного пользователя
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Проверяем, что пользователь запрашивает свои сообщения
                if (userRole == "Realtor" && userId != realtorId)
                {
                    _logger.LogWarning("Realtor {UserId} attempted to access messages of Realtor {RealtorId}", userId, realtorId);
                    return Unauthorized("You can only view your own messages");
                }
                if (userRole == "Client" && userId != clientId)
                {
                    _logger.LogWarning("Client {UserId} attempted to access messages of Client {ClientId}", userId, clientId);
                    return Unauthorized("You can only view your own messages");
                }

                // Получаем сообщения
                var messages = await _chatService.GetChatMessagesAsync(clientId, "Client", realtorId, "Realtor");
                _logger.LogInformation("{UserRole} {UserId} retrieved messages between Client {ClientId} and Realtor {RealtorId}", userRole, userId, clientId, realtorId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages between Client {ClientId} and Realtor {RealtorId}", clientId, realtorId);
                return StatusCode(500, "An error occurred while retrieving messages");
            }
        }

        [HttpPost("{realtorId}/chat/send/{clientId}")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> SendMessage(int realtorId, int clientId, [FromBody] SendMessageRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (realtorId != userId)
                {
                    _logger.LogWarning("Realtor {UserId} attempted to send message as Realtor {RequestedRealtorId}", userId, realtorId);
                    return Unauthorized("You can only send messages from your own account");
                }

                var message = new ChatMessage
                {
                    SenderId = realtorId,
                    SenderType = "Realtor",
                    ReceiverId = clientId,
                    ReceiverType = "Client",
                    Content = request.Content,
                    SentAt = DateTime.UtcNow
                };

                await _chatService.SendMessageAsync(message, clientId);
                await _hubContext.Clients.User(clientId.ToString()).SendAsync("ReceiveMessage", message);

                _logger.LogInformation("Realtor {RealtorId} sent message to Client {ClientId}", realtorId, clientId);
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
                _logger.LogError(ex, "Error sending message from Realtor {RealtorId} to Client {ClientId}", realtorId, clientId);
                return StatusCode(500, "An error occurred while sending the message");
            }
        }
    }
}
