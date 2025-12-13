using System.ComponentModel.DataAnnotations;

namespace AltaGasTest.Data.Entities
{
    public class CanadianCity
    {
        [Key]
        public int Id { get; set; }
        public required string CityName { get; set; }
        public required string TimeZone { get; set; }
    }
}
