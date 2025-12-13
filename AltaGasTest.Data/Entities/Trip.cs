using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaGasTest.Data.Entities
{
    /// <summary>
    /// Represents a trip record for equipment transportation between cities.
    /// </summary>
    public class Trip
    {
        /// <summary>
        /// Gets or sets the unique identifier for the trip.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the equipment identifier for this trip.
        /// </summary>
        public string EquipmentId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the origin city ID.
        /// </summary>
        public int OriginCityId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the origin city.
        /// This property is automatically populated from the database based on OriginCityId.
        /// </summary>
        [ForeignKey(nameof(OriginCityId))]
        public CanadianCity OriginCity { get; set; } = null!;

        /// <summary>
        /// Gets or sets the destination city ID. Can be null if trip is incomplete.
        /// </summary>
        public int? DestinationCityId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the destination city.
        /// This property is automatically populated from the database based on DestinationCityId.
        /// </summary>
        [ForeignKey(nameof(DestinationCityId))]
        public CanadianCity? DestinationCity { get; set; }

        /// <summary>
        /// Gets or sets the start time of the trip in UTC.
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Gets or sets the end time of the trip in UTC. Default value indicates incomplete trip.
        /// </summary>
        public DateTime EndUtc { get; set; }

        /// <summary>
        /// Gets or sets the total trip duration in hours.
        /// </summary>
        public double TotalTripHours { get; set; }
    }
}
