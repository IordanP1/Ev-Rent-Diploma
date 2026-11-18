using System.ComponentModel.DataAnnotations;

namespace EvRent.Models
{
    public class ChargingStation
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public string Operator { get; set; }
        [Required]
        public double PowerOutputKw { get; set; }
        [Required]
        public string ConnectorType { get; set; }
        [Required]
        public bool IsFastCharging { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string City { get; set; }
    }
}
