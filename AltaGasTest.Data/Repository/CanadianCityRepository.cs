using AltaGasTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AltaGasTest.Data.Repository
{
    public class CanadianCityRepository : ICanadianCityRepository
    {
        private readonly AltaGasTestDbContext _context;

        public CanadianCityRepository(AltaGasTestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CanadianCity>> GetAllAsync()
        {
            return await _context.CanadianCities.AsNoTracking().ToListAsync();
        }
    }
}
