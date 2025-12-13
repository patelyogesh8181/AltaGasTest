using AltaGasTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AltaGasTest.Data.Repository
{
    public class EquipmentEventRepository : IEquipmentEventRepository
    {
        private readonly AltaGasTestDbContext _context;

        public EquipmentEventRepository(AltaGasTestDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves all equipment events for the specified equipment ordered by event time (ascending).
        /// </summary>
        /// <param name="equipmentId">Equipment identifier to filter events by.</param>
        /// <returns>List of <see cref="EquipmentEvent"/> ordered by <c>EventTime</c>.</returns>
        public async Task<List<EquipmentEvent>> GetAsync(string equipmentId)
        {
            return await _context.EquipmentEvents
                .Include(e => e.City)
                .Where(e => e.EquipmentId == equipmentId)
                .OrderBy(e => e.EventTime)
                .ToListAsync();
        }

        /// <summary>
        /// Adds multiple EquipmentEvent and saves changes to the database.
        /// </summary>
        /// <param name="equipmentEvents">The EquipmentEvent to add.</param>
        public async Task AddAsync(List<EquipmentEvent> equipmentEvents)
        {
            await _context.EquipmentEvents.AddRangeAsync(equipmentEvents);
            await _context.SaveChangesAsync();
        }
    }
}
