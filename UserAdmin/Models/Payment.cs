using System.ComponentModel.DataAnnotations;

namespace EvRent.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public Rental Rental { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public string Status { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
