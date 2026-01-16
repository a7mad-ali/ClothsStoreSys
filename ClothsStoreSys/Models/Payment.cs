using System.ComponentModel.DataAnnotations;

namespace ClothsStoreSys.Models
{
    public enum PaymentType { Cash, Credit }

    public class Payment
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}