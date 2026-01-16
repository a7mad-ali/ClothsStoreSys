using System.ComponentModel.DataAnnotations;

namespace ClothsStoreSys.Models
{
    public class Return
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int CashierId { get; set; }
        public decimal Total { get; set; }

        public ICollection<ReturnItem> Items { get; set; }
    }
}