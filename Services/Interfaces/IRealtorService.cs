using RealtorConnect.Models;

namespace RealtorConnect.Services.Interfaces
{
    public interface IRealtorService
    {
        Task<List<Realtor>> GetAllRealtorsAsync();
        Task<Realtor> GetRealtorByIdAsync(int id);
        Task AddRealtorAsync(Realtor realtor);
        Task UpdateRealtorAsync(Realtor realtor);
        Task DeleteRealtorAsync(int id);
        Task<string> UploadPhotoAsync(int realtorId, IFormFile file);
    }
}