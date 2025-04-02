using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Services
{
    public class RealtorService
    {
        private readonly IRealtorRepository _repository;

        public RealtorService(IRealtorRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Realtor>> GetRealtorsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Realtor> GetRealtorByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddRealtorAsync(Realtor realtor)
        {
            await _repository.AddAsync(realtor);
        }

        public async Task UpdateRealtorAsync(Realtor realtor)
        {
            await _repository.UpdateAsync(realtor);
        }

        public async Task DeleteRealtorAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}