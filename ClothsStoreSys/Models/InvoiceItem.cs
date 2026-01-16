using System.ComponentModel.DataAnnotations;

namespace ClothsStoreSys.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public DiscountType ItemDiscountType { get; set; }
        public decimal ItemDiscountValue { get; set; }
        public decimal ItemDiscountAmount { get; set; }

        public decimal Total { get; set; }
    }
}