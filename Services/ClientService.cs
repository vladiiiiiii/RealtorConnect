using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;
using RealtorConnect.Services.Interfaces;

namespace RealtorConnect.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IRealtorRepository _realtorRepository;


        public ClientService(IClientRepository clientRepository, IRealtorRepository realtorRepository)
        {
            _clientRepository = clientRepository;
            IRealtorRepository _realtorRepository;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<List<Client>> GetClientsByGroupIdAsync(int groupId)
        {
            return await _clientRepository.GetClientsByGroupIdAsync(groupId);
        }

        public async Task<List<Client>> GetClientsByRealtorIdAsync(int realtorId)
        {
            return await _clientRepository.GetClientsByRealtorIdAsync(realtorId);
        }

        public async Task<bool> IsRealtorInGroupAsync(int realtorId, int groupId)
        {
            var realtor = await _realtorRepository.GetByIdAsync(realtorId);
            return realtor != null && realtor.GroupId == groupId;
        }

        public async Task AddClientAsync(Client client)
        {
            await _clientRepository.AddAsync(client);
        }

        public async Task UpdateClientAsync(Client client)
        {
            await _clientRepository.UpdateAsync(client);
        }

        public async Task DeleteClientAsync(int id)
        {
            await _clientRepository.DeleteAsync(id);
        }

        public async Task<string> UploadPhotoAsync(int clientId, IFormFile file)
        {
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
                throw new Exception("Client not found");

            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            client.PhotoUrl = $"/uploads/{fileName}";
            await _clientRepository.UpdateAsync(client);

            return client.PhotoUrl;
        }

        public async Task<List<Favorite>> GetFavoritesAsync(int clientId)
        {
            return await _clientRepository.GetFavoritesAsync(clientId);
        }

        public async Task AddFavoriteAsync(int clientId, int apartmentId)
        {
            var favorite = new Favorite { ClientId = clientId, ApartmentId = apartmentId };
            await _clientRepository.AddFavoriteAsync(favorite);
        }

        public async Task RemoveFavoriteAsync(int clientId, int apartmentId)
        {
            await _clientRepository.RemoveFavoriteAsync(clientId, apartmentId);
        }
    }
}