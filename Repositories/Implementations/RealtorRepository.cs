using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Repositories
{
    public class RealtorRepository : IRealtorRepository
    {
        private readonly ApplicationDbContext _context;

        public RealtorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Realtor>> GetAllAsync()
        {
            return await _context.Realtors.ToListAsync();
        }

        public async Task<Realtor> GetByIdAsync(int id)
        {
            return await _context.Realtors.FindAsync(id);
        }

        public async Task AddAsync(Realtor realtor)
        {
            _context.Realtors.Add(realtor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Realtor realtor)
        {
            _context.Realtors.Update(realtor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var realtor = await _context.Realtors.FindAsync(id);
            if (realtor != null)
            {
                _context.Realtors.Remove(realtor);
                await _context.SaveChangesAsync();
            }
        }
    }
}