using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaGasTest.Data.Entities
{
    public class EquipmentEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for the equipment.
        /// </summary>
        public string EquipmentId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the event code indicating the type of event.
        /// </summary>
        public string EventCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the event timestamp in UTC.
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Gets or sets the city ID where the event occurred.
        /// </summary>
        public int CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public CanadianCity City { get; set; } = null!;
    }
}
