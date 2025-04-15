using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ApartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Apartment>> GetAllAsync()
        {
            return await _context.Apartments
                .Include(a => a.Realtor)
                .Include(a => a.Client)
                .Include(a => a.Status)
                .Include(a => a.Favorites)
                .ToListAsync();
        }

        public async Task<Apartment> GetByIdAsync(int id)
        {
            return await _context.Apartments
                .Include(a => a.Realtor)
                .Include(a => a.Client)
                .Include(a => a.Status)
                .Include(a => a.Favorites)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Apartment apartment)
        {
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Apartment apartment)
        {
            _context.Apartments.Update(apartment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();
            }
        }
    }
}