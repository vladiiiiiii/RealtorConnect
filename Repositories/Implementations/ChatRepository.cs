using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Repositories.Implementations
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage> GetByIdAsync(int id)
        {
            return await _context.ChatMessages
                .Include(cm => cm.SenderClient)
                .Include(cm => cm.SenderRealtor)
                .FirstOrDefaultAsync(cm => cm.Id == id);
        }

        public async Task<IEnumerable<ChatMessage>> GetAllAsync()
        {
            return await _context.ChatMessages
                .Include(cm => cm.ReceiverClient)
                .Include(cm => cm.ReceiverRealtor)
                .ToListAsync();
        }

        public async Task AddAsync(ChatMessage entity)
        {
            _context.ChatMessages.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChatMessage entity)
        {
            _context.ChatMessages.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.ChatMessages.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}