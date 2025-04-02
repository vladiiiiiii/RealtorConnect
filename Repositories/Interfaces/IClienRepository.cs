using RealtorConnect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtorConnect.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<Client> GetClientByIdAsync(int id);
        Task<Client> GetClientByEmailAsync(string email);
        Task<IEnumerable<Client>> GetClientsByGroupIdAsync(int groupId);
        Task<IEnumerable<Client>> GetAllAsync();
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(int id);
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int userId, string userType, int otherUserId, string otherUserType);
    }
}
