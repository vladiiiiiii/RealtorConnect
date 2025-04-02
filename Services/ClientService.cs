using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtorConnect.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetClientByIdAsync(id);
        }

        public async Task<List<Client>> GetClientsByGroupIdAsync(int groupId)
        {
            var clients = await _clientRepository.GetClientsByGroupIdAsync(groupId);
            return clients.ToList();
        }

        public async Task<List<Client>> GetAllAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return clients.ToList();
        }

        public async Task<Client> AddClientAsync(Client client)
        {
            await _clientRepository.AddClientAsync(client);
            return client;
        }

        public async Task<Client> UpdateClientAsync(Client client)
        {
            await _clientRepository.UpdateClientAsync(client);
            return client;
        }

        public async Task DeleteClientAsync(int id)
        {
            await _clientRepository.DeleteClientAsync(id);
        }
    }
}
