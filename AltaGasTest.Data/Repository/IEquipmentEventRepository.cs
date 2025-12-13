using AltaGasTest.Data.Entities;

namespace AltaGasTest.Data.Repository
{
    public interface IEquipmentEventRepository
    {
        /// <summary>
        /// Gets all equipment events for the specified equipment ordered by event time (ascending).
        /// </summary>
        /// <param name="equipmentId">The equipment identifier to filter events by.</param>
        /// <returns>List of <see cref="EquipmentEvent"/> for the equipment.</returns>
        Task<List<EquipmentEvent>> GetAsync(string equipmentId);

        /// <summary>
        /// Adds multiple EquipmentEvent and saves changes to the database.
        /// </summary>
        /// <param name="equipmentEvents">The EquipmentEvent to add.</param>
        Task AddAsync(List<EquipmentEvent> equipmentEvents);
    }
}
