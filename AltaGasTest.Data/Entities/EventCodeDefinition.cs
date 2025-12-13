using System.ComponentModel.DataAnnotations;

namespace AltaGasTest.Data.Entities
{
    public class EventCodeDefinition
    {
        [Key]
        public int Id { get; set; } 
        public required string EventCode { get; set; }
        public required string EventDescription { get; set; }
        public required string LongDescription { get; set; }   
    }
}
