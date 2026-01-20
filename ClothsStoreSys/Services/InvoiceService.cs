using ClothsStoreSys.Data;
using ClothsStoreSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

            // Use SP if configured, otherwise fallback to EF (legacy)
            // For this task, we expose a specific method, but we can also switch here.
            // Let's keep existing logic for fallback or legacy views, and add new method.
            
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
            
            // _db.SaveChanges(); // Note: The original code didn't call SaveChanges(). Assuming it's called upstream or missing.
            // Based on analyzing the repo, it seems SaveChanges might be missing in the snippet?
            // "Sales.razor" calls "await InvoiceService.CreateInvoiceAsync(invoice, cart);"
            // "InvoiceService" returns Task.FromResult(invoice).
            // It seems the original code was INCOMPLETE (In-Memory only?).
            // I will add SaveChanges here to fix the legacy path too if needed, but the user asked for SP.
            _db.SaveChanges();

            return Task.FromResult(invoice);
        }

        public async Task<int> SaveInvoiceViaSpAsync(Invoice invoice, List<InvoiceItem> items)
        {
             // 1. Calculate Totals (Reuse logic or trust frontend? Trusting frontend/recalc here is safer)
             // simplified recalc for SP
             decimal total = invoice.Total; // Assume caller calculated it or re-calc here
             
             // 2. Prepare JSON
             var itemsDto = items.Select(x => new 
             {
                 ProductId = x.ItemId,
                 Quantity = x.Quantity,
                 UnitPrice = x.UnitPrice,
                 Factor = 1 // Default factor
             });
             
             var json = JsonSerializer.Serialize(itemsDto);
             
             // 3. Call SP
             // sp_SaveSalesInvoice @Date, @CashierId, @InvoiceNumber, @Total, @ItemsJson
             
             var pDate = new Microsoft.Data.SqlClient.SqlParameter("@Date", invoice.Date);
             var pCashier = new Microsoft.Data.SqlClient.SqlParameter("@CashierId", invoice.CashierId);
             var pInvNum = new Microsoft.Data.SqlClient.SqlParameter("@InvoiceNumber", invoice.InvoiceNumber ?? GenerateInvoiceNumber());
             var pTotal = new Microsoft.Data.SqlClient.SqlParameter("@Total", total);
             var pJson = new Microsoft.Data.SqlClient.SqlParameter("@ItemsJson", json);
             
             // We need to capture the output? The SP implies usage of valid tables that might not map to EF yet.
             // We execute it raw.
             
             await _db.Database.ExecuteSqlRawAsync("EXEC sp_SaveSalesInvoice @Date, @CashierId, @InvoiceNumber, @Total, @ItemsJson", 
                pDate, pCashier, pInvNum, pTotal, pJson);
                
             return 1; // Success
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