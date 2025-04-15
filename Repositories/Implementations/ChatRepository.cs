using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetChatMessagesAsync(int senderId, string senderType, int receiverId, string receiverType)
        {
            return await _context.ChatMessages
                .Where(m => (m.SenderId == senderId && m.SenderType == senderType && m.ReceiverId == receiverId && m.ReceiverType == receiverType) ||
                            (m.SenderId == receiverId && m.SenderType == receiverType && m.ReceiverId == senderId && m.ReceiverType == senderType))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}