using ClothsStoreSys.Data;
using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _db;
        public InvoiceService(AppDbContext db) { _db = db; }

        public Task<Invoice> CreateInvoiceAsync(Invoice invoice, List<InvoiceItem> items)
        {
            decimal subtotal = 0;
            foreach (var it in items)
            {
                if (it.ItemDiscountType == DiscountType.Percent)
                {
                    it.ItemDiscountAmount = it.UnitPrice * it.Quantity * (it.ItemDiscountValue / 100);
                }
                else if (it.ItemDiscountType == DiscountType.Fixed)
                {
                    it.ItemDiscountAmount = it.ItemDiscountValue * it.Quantity;
                }
                else it.ItemDiscountAmount = 0;

                it.Total = (it.UnitPrice * it.Quantity) - it.ItemDiscountAmount;
                subtotal += it.Total;
            }

            invoice.Subtotal = subtotal;

            if (invoice.InvoiceDiscountType == DiscountType.Percent)
            {
                invoice.InvoiceDiscountAmount = subtotal * (invoice.InvoiceDiscountValue / 100);
            }
            else if (invoice.InvoiceDiscountType == DiscountType.Fixed)
            {
                invoice.InvoiceDiscountAmount = invoice.InvoiceDiscountValue;
            }
            else invoice.InvoiceDiscountAmount = 0;

            invoice.Total = subtotal - invoice.InvoiceDiscountAmount;

            invoice.Id = (_db.Invoices.LastOrDefault()?.Id ?? 0) + 1;
            invoice.InvoiceNumber = GenerateInvoiceNumber();
            invoice.Date = DateTime.UtcNow;

            _db.Invoices.Add(invoice);

            foreach (var it in items)
            {
                it.Id = (_db.InvoiceItems.LastOrDefault()?.Id ?? 0) + 1;
                it.InvoiceId = invoice.Id;
                _db.InvoiceItems.Add(it);

                var item = _db.Items.FirstOrDefault(i => i.Id == it.ItemId);
                if (item != null)
                {
                    item.StockQty -= it.Quantity;
                }
            }

            return Task.FromResult(invoice);
        }

        public Task<Invoice> GetByIdAsync(int id)
        {
            var inv = _db.Invoices.FirstOrDefault(i => i.Id == id);
            if (inv != null)
            {
                inv.Items = _db.InvoiceItems.Where(ii => ii.InvoiceId == inv.Id).ToList();
                foreach (var ii in inv.Items)
                {
                    ii.Item = _db.Items.FirstOrDefault(it => it.Id == ii.ItemId);
                }
            }
            return Task.FromResult(inv);
        }

        public Task<List<Invoice>> GetAllAsync()
        {
            return Task.FromResult(_db.Invoices.OrderByDescending(i => i.Date).ToList());
        }

        private string GenerateInvoiceNumber()
        {
            var rnd = new Random();
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{rnd.Next(1000, 9999)}";
        }
    }
}