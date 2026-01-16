using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public class InvoiceLine
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public DiscountType ItemDiscountType { get; set; }
        public decimal ItemDiscountValue { get; set; }
        public decimal ItemDiscountAmount { get; set; }
        public decimal Total { get; set; }
    }

    public class CurrentInvoiceService
    {
        public List<InvoiceLine> Lines { get; } = new List<InvoiceLine>();

        public DiscountType InvoiceDiscountType { get; set; } = DiscountType.None;
        public decimal InvoiceDiscountValue { get; set; }

        public PaymentType PaymentType { get; set; } = PaymentType.Cash;
        public decimal PaidAmount { get; set; }

        public void Clear()
        {
            Lines.Clear();
            InvoiceDiscountType = DiscountType.None;
            InvoiceDiscountValue = 0;
            PaymentType = PaymentType.Cash;
            PaidAmount = 0;
        }

        public void AddItem(Item item)
        {
            var line = Lines.FirstOrDefault(l => l.ItemId == item.Id);
            if (line == null)
            {
                Lines.Add(new InvoiceLine
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    UnitPrice = item.UnitPrice,
                    Quantity = 1,
                    ItemDiscountType = DiscountType.None,
                    ItemDiscountValue = 0
                });
            }
            else
            {
                line.Quantity += 1;
            }

            RecalculateLineTotals();
        }

        public void RemoveItem(InvoiceLine line)
        {
            Lines.Remove(line);
            RecalculateLineTotals();
        }

        public void SetQuantity(InvoiceLine line, int qty)
        {
            if (qty <= 0) return;
            line.Quantity = qty;
            RecalculateLineTotals();
        }

        public void SetItemDiscount(InvoiceLine line, DiscountType type, decimal value)
        {
            line.ItemDiscountType = type;
            line.ItemDiscountValue = value;
            RecalculateLineTotals();
        }

        public void RecalculateLineTotals()
        {
            foreach (var l in Lines)
            {
                if (l.ItemDiscountType == DiscountType.Percent)
                {
                    l.ItemDiscountAmount = l.UnitPrice * l.Quantity * (l.ItemDiscountValue / 100);
                }
                else if (l.ItemDiscountType == DiscountType.Fixed)
                {
                    l.ItemDiscountAmount = l.ItemDiscountValue * l.Quantity;
                }
                else l.ItemDiscountAmount = 0;

                l.Total = (l.UnitPrice * l.Quantity) - l.ItemDiscountAmount;
            }
        }

        public decimal Subtotal => Lines.Sum(l => l.Total);

        public decimal InvoiceDiscountAmount
        {
            get
            {
                if (InvoiceDiscountType == DiscountType.Percent) return Subtotal * (InvoiceDiscountValue / 100);
                if (InvoiceDiscountType == DiscountType.Fixed) return InvoiceDiscountValue;
                return 0m;
            }
        }

        public decimal Total => Subtotal - InvoiceDiscountAmount;

        public decimal Remaining => Total - PaidAmount;
    }
}
