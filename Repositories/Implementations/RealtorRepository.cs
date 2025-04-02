using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;


namespace RealtorConnect.Repositories.Implementations
{
    public class RealtorRepository : IRealtorRepository
    {
        private readonly ApplicationDbContext _context;

        public RealtorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Realtor> GetByIdAsync(int id)
        {
            return await _context.Realtors
                .Include(r => r.Apartments)
                .Include(r => r.GroupRealtors)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Realtor>> GetAllAsync()
        {
            return await _context.Realtors
                .Include(r => r.Apartments)
                .Include(r => r.GroupRealtors)
                .ToListAsync();
        }

        public async Task AddAsync(Realtor entity)
        {
            _context.Realtors.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Realtor entity)
        {
            _context.Realtors.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Realtors.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
    public class ChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessageAsync(int senderId, string senderType, int receiverId, string receiverType, string messageContent)
        {
            var chatMessage = new ChatMessage
            {
                MessageContent = messageContent,
                SentAt = DateTime.UtcNow
            };

            if (senderType == "Client")
                chatMessage.SenderClientId = senderId;
            else if (senderType == "Realtor")
                chatMessage.SenderRealtorId = senderId;

            if (receiverType == "Client")
                chatMessage.ReceiverClientId = receiverId;
            else if (receiverType == "Realtor")
                chatMessage.ReceiverRealtorId = receiverId;

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(int userId, string userType)
        {
            var query = _context.ChatMessages.AsQueryable();

            if (userType == "Client")
            {
                query = query.Where(cm => cm.SenderClientId == userId || cm.ReceiverClientId == userId);
            }
            else if (userType == "Realtor")
            {
                query = query.Where(cm => cm.SenderRealtorId == userId || cm.ReceiverRealtorId == userId);
            }

            return await query
                .OrderBy(cm => cm.SentAt)
                .ToListAsync();
        }
    }
}