using Microsoft.EntityFrameworkCore;
using ZaitovaLibrary.Models;

namespace ZaitovaLibrary.Data
{
    public interface IPartnerTypeRepository
    {
        Task<List<PartnerType>> GetAllAsync();
        Task<PartnerType?> GetByIdAsync(int id);
    }

    public class PartnerTypeRepository : IPartnerTypeRepository
    {
        private readonly AppDbContext _context;

        public PartnerTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PartnerType>> GetAllAsync()
        {
            return await _context.PartnerTypes
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<PartnerType?> GetByIdAsync(int id)
        {
            return await _context.PartnerTypes.FindAsync(id);
        }
    }
}