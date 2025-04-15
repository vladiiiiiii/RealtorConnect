using RealtorConnect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtorConnect.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<List<Client>> GetClientsByGroupIdAsync(int groupId);
        Task<List<Client>> GetAllAsync();
        Task<Client> GetByIdAsync(int id);
        Task<List<Client>> GetClientsByRealtorIdAsync(int realtorId);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);
        Task<List<Favorite>> GetFavoritesAsync(int clientId);
        Task AddFavoriteAsync(Favorite favorite);
        Task RemoveFavoriteAsync(int clientId, int apartmentId);
    }
}