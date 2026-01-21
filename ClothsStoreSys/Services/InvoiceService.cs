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

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, List<InvoiceItem> items)
        {
            // استخدام الـ Transaction لضمان (حفظ الكل أو لا شيء)
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1. توليد رقم الفاتورة تلقائياً إذا لم يكن موجوداً
                if (string.IsNullOrEmpty(invoice.InvoiceNumber))
                    invoice.InvoiceNumber = GenerateInvoiceNumber(); //

                // 2. ربط العناصر بالفاتورة
                invoice.Items = items; //

                // 3. تحديث المخزن لكل صنف
                foreach (var it in items)
                {
                    var dbItem = await _db.Items.FindAsync(it.ItemId);
                    if (dbItem != null)
                    {
                        dbItem.StockQty -= it.Quantity; //
                        _db.Entry(dbItem).State = EntityState.Modified;
                    }
                }

                _db.Invoices.Add(invoice); //
                await _db.SaveChangesAsync();

                await tx.CommitAsync(); //
                return invoice;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<int> SaveInvoiceViaSpAsync(Invoice invoice, List<InvoiceItem> items)
        {
             decimal total = invoice.Total;
             var itemsDto = items.Select(x => new 
             {
                 ProductId = x.ItemId,
                 Quantity = x.Quantity,
                 UnitPrice = x.UnitPrice,
                 Factor = 1
             });
             var json = JsonSerializer.Serialize(itemsDto);
             var pDate = new Microsoft.Data.SqlClient.SqlParameter("@Date", invoice.Date);
             var pCashier = new Microsoft.Data.SqlClient.SqlParameter("@CashierId", invoice.CashierId);
             var pInvNum = new Microsoft.Data.SqlClient.SqlParameter("@InvoiceNumber", invoice.InvoiceNumber ?? GenerateInvoiceNumber());
             var pTotal = new Microsoft.Data.SqlClient.SqlParameter("@Total", total);
             var pJson = new Microsoft.Data.SqlClient.SqlParameter("@ItemsJson", json);
             await _db.Database.ExecuteSqlRawAsync("EXEC sp_SaveSalesInvoice @Date, @CashierId, @InvoiceNumber, @Total, @ItemsJson", 
                pDate, pCashier, pInvNum, pTotal, pJson);
             return 1;
        }

        public Task<Invoice> GetByIdAsync(int id)
        {
            var inv = _db.Invoices.Include(i => i.Items).ThenInclude(ii => ii.Item).FirstOrDefault(i => i.Id == id);
            return Task.FromResult(inv);
        }

        public Task<List<Invoice>> GetAllAsync()
        {
            return Task.FromResult(_db.Invoices.Include(i => i.Items).OrderByDescending(i => i.Date).ToList());
        }

        private string GenerateInvoiceNumber()
        {
            var rnd = new Random();
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{rnd.Next(1000, 9999)}";
        }
    }
}