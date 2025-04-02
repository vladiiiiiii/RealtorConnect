using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtorConnect.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<Client> GetClientByEmailAsync(string email)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        // Исправлено имя метода для соответствия интерфейсу
        public async Task<IEnumerable<Client>> GetClientsByGroupIdAsync(int groupId)
        {
            return await _context.Clients
                .Where(c => c.GroupClientId == groupId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task AddClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int userId, string userType, int otherUserId, string otherUserType)
        {
            var query = _context.ChatMessages.AsQueryable();

            if (userType == "Client")
            {
                if (otherUserType == "Client")
                {
                    query = query.Where(m =>
                        (m.SenderClientId == userId && m.ReceiverClientId == otherUserId) ||
                        (m.SenderClientId == otherUserId && m.ReceiverClientId == userId));
                }
                else // otherUserType == "Realtor"
                {
                    query = query.Where(m =>
                        (m.SenderClientId == userId && m.ReceiverRealtorId == otherUserId) ||
                        (m.SenderRealtorId == otherUserId && m.ReceiverClientId == userId));
                }
            }
            else // userType == "Realtor"
            {
                if (otherUserType == "Client")
                {
                    query = query.Where(m =>
                        (m.SenderRealtorId == userId && m.ReceiverClientId == otherUserId) ||
                        (m.SenderClientId == otherUserId && m.ReceiverRealtorId == userId));
                }
                else // otherUserType == "Realtor"
                {
                    query = query.Where(m =>
                        (m.SenderRealtorId == userId && m.ReceiverRealtorId == otherUserId) ||
                        (m.SenderRealtorId == otherUserId && m.ReceiverRealtorId == userId));
                }
            }

            return await query
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessage
                {
                    Id = m.Id,
                    SenderClientId = m.SenderClientId,
                    SenderRealtorId = m.SenderRealtorId,
                    ReceiverClientId = m.ReceiverClientId,
                    ReceiverRealtorId = m.ReceiverRealtorId,
                    MessageContent = m.MessageContent,
                    SentAt = m.SentAt,
                    SenderClient = m.SenderClient,
                    SenderRealtor = m.SenderRealtor,
                    ReceiverClient = m.ReceiverClient,
                    ReceiverRealtor = m.ReceiverRealtor
                })
                .ToListAsync();
        }
    }
}
