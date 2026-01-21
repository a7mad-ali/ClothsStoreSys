using System.ComponentModel.DataAnnotations;

namespace ClothsStoreSys.Models
{
    public enum InvoiceStatus { Paid, Partial, Credit }
    public enum DiscountType { None, Fixed, Percent }

    public class Invoice
    {
        public int Id { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int CashierId { get; set; }
        public InvoiceStatus Status { get; set; }

        // Totals stored
        public decimal Subtotal { get; set; }
        public DiscountType InvoiceDiscountType { get; set; }
        public decimal InvoiceDiscountValue { get; set; }
        public decimal InvoiceDiscountAmount { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }

        public ICollection<InvoiceItem> Items { get; set; }
    }
}

