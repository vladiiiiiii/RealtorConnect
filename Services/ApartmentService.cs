using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;


namespace RealtorConnect.Services
{
    public class ApartmentService
    {
        private readonly IApartmentRepository _repository;

        public ApartmentService(IApartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Apartment>> GetApartmentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Apartment> GetApartmentByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddApartmentAsync(Apartment apartment)
        {
            await _repository.AddAsync(apartment);
        }

        public async Task UpdateApartmentAsync(Apartment apartment)
        {
            await _repository.UpdateAsync(apartment);
        }

        public async Task DeleteApartmentAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}