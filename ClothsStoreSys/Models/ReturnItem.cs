using System.ComponentModel.DataAnnotations;

namespace ClothsStoreSys.Models
{
    public class ReturnItem
    {
        public int Id { get; set; }
        public int ReturnId { get; set; }
        public Return Return { get; set; }

        public int InvoiceItemId { get; set; }
        public InvoiceItem InvoiceItem { get; set; }

        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}