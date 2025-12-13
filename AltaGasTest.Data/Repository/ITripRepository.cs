using AltaGasTest.Data.Entities;

namespace AltaGasTest.Data.Repository
{
    /// <summary>
    /// Repository interface for Trip entity operations with eager loading of related cities.
    /// All query methods automatically include OriginCity and DestinationCity navigation properties.
    /// </summary>
    public interface ITripRepository
    {
        /// <summary>
        /// Gets all trips with related city data eager-loaded.
        /// </summary>
        /// <returns>List of all trips ordered by start time.</returns>
        Task<List<Trip>> GetAllAsync();

        /// <summary>
        /// Adds multiple trips and saves changes to the database.
        /// </summary>
        /// <param name="trips">The trips to add.</param>
        Task AddAsync(List<Trip> trips);
    }
}
