using System.ComponentModel.DataAnnotations;
namespace EvRent.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        [Range(2015,2026)]
        public int Year { get; set; }

        [Range(2,7)]
        public int Seats { get; set; }

        [Required]
        public string Category { get; set; }

        [Range(0,1000)]
        public double BatteryCapacityKwh { get; set; }

        [Range(0, 100)]
        public int BatteryLevelPercent { get; set; }
        [Range(1,100)]
        public double AverageConsumptionKwhPer100km { get; set; }

        [Required]
        public int OfficialRangeKm { get; set; }

        [Required]
        public string ConnectorType { get; set; }

        [Required]
        public bool IsFastChargingSupported { get; set; }
        
        [Required]
        public decimal PricePerDay { get; set; }

        public string ImageUrl { get; set; }


        [Required]
        public string RegistationNumber { get; set; }

        [Required]
        public string Status { get; set; }

        public string? Description { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
