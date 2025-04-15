using RealtorConnect.Models;

namespace RealtorConnect.Services.Interfaces
{
    public interface IApartmentService
    {
        Task<List<Apartment>> GetAllApartmentsAsync();
        Task<Apartment> GetApartmentByIdAsync(int id);
        Task AddApartmentAsync(Apartment apartment, int userId, string userRole);
        Task UpdateApartmentAsync(Apartment apartment, int userId, string userRole);
        Task DeleteApartmentAsync(int id, int userId, string userRole);
        Task<string> UploadPhotoAsync(int apartmentId, IFormFile file, int userId, string userRole);
    }
}