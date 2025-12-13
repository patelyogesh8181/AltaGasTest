using AltaGasTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AltaGasTest.Data.Repository
{
    /// <summary>
    /// Repository for managing Trip entities with eager loading of related cities.
    /// </summary>
    public class TripRepository : ITripRepository
    {
        private readonly AltaGasTestDbContext _context;

        public TripRepository(AltaGasTestDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets all trips with related city data eager-loaded.
        /// </summary>
        /// <returns>List of all trips ordered by start time.</returns>
        public async Task<List<Trip>> GetAllAsync()
        {
            return await _context.Trips
                .Include(t => t.OriginCity)
                .Include(t => t.DestinationCity)
                .OrderBy(t => t.StartUtc)
                .ToListAsync();
        }

        /// <summary>
        /// Adds multiple trips and saves changes to the database.
        /// </summary>
        /// <param name="trips">The trips to add.</param>
        public async Task AddAsync(List<Trip> trips)
        {
            await _context.Trips.AddRangeAsync(trips);
            await _context.SaveChangesAsync();
        }
    }
}
