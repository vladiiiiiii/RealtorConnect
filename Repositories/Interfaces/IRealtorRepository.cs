using RealtorConnect.Models;

namespace RealtorConnect.Repositories.Interfaces
{
    public interface IRealtorRepository
    {
        Task<List<Realtor>> GetAllAsync();
        Task<Realtor> GetByIdAsync(int id);
        Task AddAsync(Realtor realtor);
        Task UpdateAsync(Realtor realtor);
        Task DeleteAsync(int id);
    }
}