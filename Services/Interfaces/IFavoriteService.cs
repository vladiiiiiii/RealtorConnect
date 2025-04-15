using RealtorConnect.Models;

namespace RealtorConnect.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task AddToFavoritesAsync(int clientId, int apartmentId);
        Task RemoveFromFavoritesAsync(int clientId, int apartmentId);
        Task<IEnumerable<Favorite>> GetFavoritesAsync(int clientId);
        Task<bool> IsFavoriteAsync(int clientId, int apartmentId);
    }
}