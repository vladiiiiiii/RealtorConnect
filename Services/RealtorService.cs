using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;
using RealtorConnect.Services.Interfaces;

namespace RealtorConnect.Services
{
    public class RealtorService : IRealtorService
    {
        private readonly IRealtorRepository _realtorRepository;
        private readonly ApplicationDbContext _context;

        public RealtorService(IRealtorRepository realtorRepository, ApplicationDbContext context)
        {
            _realtorRepository = realtorRepository;
            _context = context;
        }

        public async Task<List<Realtor>> GetAllRealtorsAsync()
        {
            return await _realtorRepository.GetAllAsync();
        }

        public async Task<Realtor> GetRealtorByIdAsync(int id)
        {
            return await _realtorRepository.GetByIdAsync(id);
        }

        public async Task AddRealtorAsync(Realtor realtor)
        {
            await _realtorRepository.AddAsync(realtor);
        }

        public async Task UpdateRealtorAsync(Realtor realtor)
        {
            await _realtorRepository.UpdateAsync(realtor);
        }

        public async Task DeleteRealtorAsync(int id)
        {
            await _realtorRepository.DeleteAsync(id);
        }

        public async Task<string> UploadPhotoAsync(int realtorId, IFormFile file)
        {
            var realtor = await _context.Realtors.FindAsync(realtorId);
            if (realtor == null)
                throw new Exception("Realtor not found");

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

            realtor.PhotoUrl = $"/uploads/{fileName}";
            await _context.SaveChangesAsync();

            return realtor.PhotoUrl;
        }
    }
}