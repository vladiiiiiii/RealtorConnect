using RealtorConnect.Models;

namespace RealtorConnect.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterRealtorAsync(Realtor realtor);
        Task<bool> RegisterClientAsync(Client client);
        Task<(string Token, string Role, int Id)?> LoginAsync(string email, string password);
    }
}