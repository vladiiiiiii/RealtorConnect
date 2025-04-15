using RealtorConnect.Models;

namespace RealtorConnect.Repositories.Interfaces
{
    public interface IApartmentRepository
    {
        Task<List<Apartment>> GetAllAsync();
        Task<Apartment> GetByIdAsync(int id);
        Task AddAsync(Apartment apartment);
        Task UpdateAsync(Apartment apartment);
        Task DeleteAsync(int id);
    }
}