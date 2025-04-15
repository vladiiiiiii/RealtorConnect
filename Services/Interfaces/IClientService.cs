using RealtorConnect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtorConnect.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();
        Task<Client> GetClientByIdAsync(int id);
        Task<List<Client>> GetClientsByGroupIdAsync(int groupId);
        Task<List<Client>> GetClientsByRealtorIdAsync(int realtorId);
        Task<bool> IsRealtorInGroupAsync(int realtorId, int groupId);
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(int id);
        Task<string> UploadPhotoAsync(int clientId, IFormFile file);
        Task<List<Favorite>> GetFavoritesAsync(int clientId);
        Task AddFavoriteAsync(int clientId, int apartmentId);
        Task RemoveFavoriteAsync(int clientId, int apartmentId);
    }
}