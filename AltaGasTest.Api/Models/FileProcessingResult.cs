using AltaGasTest.Data.Entities;

namespace AltaGasTest.Api.Models
{
    /// <summary>
    /// Represents the result of processing an equipment events file.
    /// </summary>
    public class FileProcessingResult
    {
        /// <summary>
        /// Gets or sets the processing result message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of trips created.
        /// </summary>
        public int TripCount { get; set; }

        /// <summary>
        /// Gets or sets the list of created trips.
        /// </summary>
        public List<Trip> Trips { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of created equipment events.
        /// </summary>
        public List<EquipmentEvent> EquipmentEvents { get; set; } = new();
    }
}
