using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Hubs;
using RealtorConnect.Models;
using System.Security.Claims;
using RealtorConnect.Hubs;

namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatController(ApplicationDbContext context, IHubContext<ChatHub> chatHubContext)
        {
            _context = context;
            _chatHubContext = chatHubContext;
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userType = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userType == "Client" && !_context.Clients.Any(c => c.Id == userId))
                return BadRequest("Sender not found.");
            if (userType == "Realtor" && !_context.Realtors.Any(r => r.Id == userId))
                return BadRequest("Sender not found.");

            if (model.ReceiverType == "Client" && !_context.Clients.Any(c => c.Id == model.ReceiverId))
                return BadRequest("Receiver not found.");
            if (model.ReceiverType == "Realtor" && !_context.Realtors.Any(r => r.Id == model.ReceiverId))
                return BadRequest("Receiver not found.");

            var message = new ChatMessage
            {
                MessageContent = model.MessageContent,
                SentAt = DateTime.UtcNow
            };

            if (userType == "Client")
                message.SenderClientId = userId;
            else
                message.SenderRealtorId = userId;

            if (model.ReceiverType == "Client")
                message.ReceiverClientId = model.ReceiverId;
            else
                message.ReceiverRealtorId = model.ReceiverId;

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            await _chatHubContext.Clients.Group(GetChatGroupName(userId, model.ReceiverId))
                .SendAsync("ReceiveMessage", userId, userType, model.ReceiverId, model.ReceiverType, model.MessageContent);

            return Ok(new { Message = "Message sent successfully", MessageId = message.Id });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory(int otherUserId, string otherUserType)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userType = User.FindFirst(ClaimTypes.Role)?.Value;

            if (otherUserType == "Client" && !_context.Clients.Any(c => c.Id == otherUserId))
                return BadRequest("User not found.");
            if (otherUserType == "Realtor" && !_context.Realtors.Any(r => r.Id == otherUserId))
                return BadRequest("User not found.");

            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderClientId == userId && m.ReceiverClientId == otherUserId) ||
                    (m.SenderClientId == userId && m.ReceiverRealtorId == otherUserId) ||
                    (m.SenderRealtorId == userId && m.ReceiverClientId == otherUserId) ||
                    (m.SenderRealtorId == userId && m.ReceiverRealtorId == otherUserId) ||
                    (m.SenderClientId == otherUserId && m.ReceiverClientId == userId) ||
                    (m.SenderClientId == otherUserId && m.ReceiverRealtorId == userId) ||
                    (m.SenderRealtorId == otherUserId && m.ReceiverClientId == userId) ||
                    (m.SenderRealtorId == otherUserId && m.ReceiverRealtorId == userId))
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Id,
                    SenderId = m.SenderClientId ?? m.SenderRealtorId,
                    SenderType = m.SenderClientId.HasValue ? "Client" : "Realtor",
                    ReceiverId = m.ReceiverClientId ?? m.ReceiverRealtorId,
                    ReceiverType = m.ReceiverClientId.HasValue ? "Client" : "Realtor",
                    m.MessageContent,
                    m.SentAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        private string GetChatGroupName(int userId1, int userId2)
        {
            int smallerId = Math.Min(userId1, userId2);
            int largerId = Math.Max(userId1, userId2);
            return $"Chat_{smallerId}_{largerId}";
        }
    }

    public class SendMessageRequest
    {
        public int ReceiverId { get; set; }
        public string ReceiverType { get; set; }
        public string MessageContent { get; set; }
    }
}