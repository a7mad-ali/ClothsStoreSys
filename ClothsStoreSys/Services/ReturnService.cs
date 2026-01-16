using ClothsStoreSys.Data;
using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public class ReturnService : IReturnService
    {
        private readonly AppDbContext _db;
        public ReturnService(AppDbContext db) { _db = db; }

        public Task<Return> ProcessReturnAsync(Return ret, List<ReturnItem> items)
        {
            ret.Id = (_db.Returns.LastOrDefault()?.Id ?? 0) + 1;
            ret.Date = DateTime.UtcNow;
            _db.Returns.Add(ret);

            decimal total = 0;
            foreach (var ri in items)
            {
                ri.Id = (_db.ReturnItems.LastOrDefault()?.Id ?? 0) + 1;
                ri.ReturnId = ret.Id;
                var invoiceItem = _db.InvoiceItems.FirstOrDefault(ii => ii.Id == ri.InvoiceItemId);
                if (invoiceItem == null) continue;
                var unitPaid = (invoiceItem.Total / invoiceItem.Quantity);
                ri.Amount = unitPaid * ri.Quantity;
                total += ri.Amount;

                _db.ReturnItems.Add(ri);

                var item = _db.Items.FirstOrDefault(i => i.Id == invoiceItem.ItemId);
                if (item != null)
                {
                    item.StockQty += ri.Quantity;
                }
            }

            ret.Total = total;
            return Task.FromResult(ret);
        }
    }
}