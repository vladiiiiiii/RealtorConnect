using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Repositories.Implementations
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ApartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Apartment> GetByIdAsync(int id)
        {
            return await _context.Apartments
                .Include(a => a.Status)
                .Include(a => a.Realtor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Apartment>> GetAllAsync()
        {
            return await _context.Apartments
                .Include(a => a.Status)
                .Include(a => a.Realtor)
                .ToListAsync();
        }

        public async Task AddAsync(Apartment entity)
        {
            _context.Apartments.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Apartment entity)
        {
            _context.Apartments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Apartments.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}