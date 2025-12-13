using AltaGasTest.Data.Entities;

namespace AltaGasTest.Api.Services
{
    /// <summary>
    /// Service for processing trip-related equipment events and generating trip records.
    /// </summary>
    public interface ITripServices
    {
        /// <summary>
        /// Processes equipment events from a CSV file and generates trip records.
        /// </summary>
        /// <param name="file">CSV file containing equipment events.</param>
        /// <returns>List of processed trips.</returns>
        /// <exception cref="ArgumentNullException">Thrown when file is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when CSV format is invalid.</exception>
        Task<List<EquipmentEvent>> ProcessEquipmentEventsAsync(IFormFile file);

        Task<List<Trip>> BuildTripsFromEvents(List<EquipmentEvent> events);
    }
}
