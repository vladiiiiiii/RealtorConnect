using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Services.Interfaces;

namespace RealtorConnect.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddToFavoritesAsync(int clientId, int apartmentId)
        {
            // Проверяем, существует ли клиент
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
                throw new Exception("Client not found");

            // Проверяем, существует ли квартира
            var apartment = await _context.Apartments.FindAsync(apartmentId);
            if (apartment == null)
                throw new Exception("Apartment not found");

            // Проверяем, не добавлена ли квартира уже в избранное
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.ClientId == clientId && f.ApartmentId == apartmentId);
            if (existingFavorite != null)
                throw new Exception("Apartment is already in favorites");

            var favorite = new Favorite
            {
                ClientId = clientId,
                ApartmentId = apartmentId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFavoritesAsync(int clientId, int apartmentId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.ClientId == clientId && f.ApartmentId == apartmentId);
            if (favorite == null)
                throw new Exception("Apartment is not in favorites");

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Favorite>> GetFavoritesAsync(int clientId)
        {
            return await _context.Favorites
                .Include(f => f.Apartment)
                    .ThenInclude(a => a.Realtor)
                .Include(f => f.Apartment)
                    .ThenInclude(a => a.Client)
                .Include(f => f.Apartment)
                    .ThenInclude(a => a.Status)
                .Where(f => f.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(int clientId, int apartmentId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.ClientId == clientId && f.ApartmentId == apartmentId);
        }
    }
}