using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetClientsByGroupIdAsync(int groupId)
        {
            return await _context.Clients
                .Where(c => _context.Realtors
                    .Where(r => r.GroupId == groupId)
                    .Any(r => r.Id == c.CreatedByRealtorId ||
                              _context.Apartments.Any(a => a.RealtorId == r.Id && a.ClientId == c.Id)))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Client>> GetAllAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<List<Client>> GetClientsByRealtorIdAsync(int realtorId)
        {
            return await _context.Clients
                .Where(c => c.CreatedByRealtorId == realtorId ||
                            _context.Apartments.Any(a => a.RealtorId == realtorId && a.ClientId == c.Id))
                .Distinct()
                .ToListAsync();
        }

        public async Task AddAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                // Удаляем связанные данные (сообщения в чате)
                var messages = await _context.ChatMessages
                    .Where(m => (m.SenderId == id && m.SenderType == "Client") ||
                                (m.ReceiverId == id && m.ReceiverType == "Client"))
                    .ToListAsync();
                _context.ChatMessages.RemoveRange(messages);

                // Удаляем связи с квартирами
                var apartments = await _context.Apartments
                    .Where(a => a.ClientId == id)
                    .ToListAsync();
                foreach (var apartment in apartments)
                {
                    apartment.ClientId = null; // Разрываем связь
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Favorite>> GetFavoritesAsync(int clientId)
        {
            return await _context.Favorites
                .Include(f => f.Apartment)
                .Where(f => f.ClientId == clientId)
                .ToListAsync();
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(int clientId, int apartmentId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.ClientId == clientId && f.ApartmentId == apartmentId);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }
    }
}