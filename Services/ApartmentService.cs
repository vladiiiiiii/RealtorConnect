using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;
using RealtorConnect.Services.Interfaces;

namespace RealtorConnect.Services
{
    public class ApartmentService : IApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly ApplicationDbContext _context;

        public ApartmentService(IApartmentRepository apartmentRepository, ApplicationDbContext context)
        {
            _apartmentRepository = apartmentRepository;
            _context = context;
        }

        public async Task<List<Apartment>> GetAllApartmentsAsync()
        {
            return await _apartmentRepository.GetAllAsync();
        }

        public async Task<Apartment> GetApartmentByIdAsync(int id)
        {
            return await _apartmentRepository.GetByIdAsync(id);
        }

        public async Task AddApartmentAsync(Apartment apartment, int userId, string userRole)
        {
            if (userRole == "Client")
            {
                // Клиент может добавлять только свои квартиры
                apartment.ClientId = userId;
                apartment.RealtorId = null;
            }
            else if (userRole == "Realtor")
            {
                // Риэлтор может указывать ClientId, но должен быть владельцем
                apartment.RealtorId = userId;
            }

            await _apartmentRepository.AddAsync(apartment);
        }

        public async Task UpdateApartmentAsync(Apartment apartment, int userId, string userRole)
        {
            var existingApartment = await _context.Apartments.FindAsync(apartment.Id);
            if (existingApartment == null)
                throw new Exception("Apartment not found");

            if (userRole == "Client" && existingApartment.ClientId != userId)
                throw new Exception("You can only edit your own apartments");

            if (userRole == "Realtor" && existingApartment.RealtorId != userId)
                throw new Exception("You can only edit apartments you own");

            existingApartment.Address = apartment.Address;
            existingApartment.Rooms = apartment.Rooms;
            existingApartment.Area = apartment.Area;
            existingApartment.Price = apartment.Price;
            existingApartment.Description = apartment.Description;
            existingApartment.StatusId = apartment.StatusId;

            if (userRole == "Realtor")
            {
                existingApartment.ClientId = apartment.ClientId; // Риэлтор может менять клиента
            }

            await _apartmentRepository.UpdateAsync(existingApartment);
        }

        public async Task DeleteApartmentAsync(int id, int userId, string userRole)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
                throw new Exception("Apartment not found");

            if (userRole == "Client" && apartment.ClientId != userId)
                throw new Exception("You can only delete your own apartments");

            if (userRole == "Realtor" && apartment.RealtorId != userId)
                throw new Exception("You can only delete apartments you own");

            await _apartmentRepository.DeleteAsync(id);
        }

        public async Task<string> UploadPhotoAsync(int apartmentId, IFormFile file, int userId, string userRole)
        {
            var apartment = await _context.Apartments.FindAsync(apartmentId);
            if (apartment == null)
                throw new Exception("Apartment not found");

            if (userRole == "Client" && apartment.ClientId != userId)
                throw new Exception("You can only upload photos for your own apartments");

            if (userRole == "Realtor" && apartment.RealtorId != userId)
                throw new Exception("You can only upload photos for apartments you own");

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

            apartment.PhotoUrl = $"/uploads/{fileName}";
            await _context.SaveChangesAsync();

            return apartment.PhotoUrl;
        }
    }
}