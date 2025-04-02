using RealtorConnect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtorConnect.Services
{
    public interface IClientService
    {
        Task<Client> GetClientByIdAsync(int id);
        Task<List<Client>> GetClientsByGroupIdAsync(int groupId);
        Task<List<Client>> GetAllAsync();
        Task<Client> AddClientAsync(Client client);
        Task<Client> UpdateClientAsync(Client client);
        Task DeleteClientAsync(int id);
    }
}
