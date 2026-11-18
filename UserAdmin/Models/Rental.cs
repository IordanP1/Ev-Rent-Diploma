
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
namespace EvRent.Models
{
    public class Rental
    {
        public int Id { get; set; }
        [Required]
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        
        public int TotalDays { get; set; }
        public decimal BasePrice { get; set; }
        public decimal InsurancePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public string Status { get; set; } = "pending";
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; }
        public string? Notes { get; set; }
        public Payment? Payment { get; set; }
    }
}
