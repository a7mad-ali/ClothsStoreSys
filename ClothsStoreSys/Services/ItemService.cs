using ClothsStoreSys.Data;
using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _db;
        public ItemService(AppDbContext db) { _db = db; }

        public Task<List<Item>> GetAllAsync()
        {
            var list = _db.Items.Where(i => i.IsEnabled).ToList();
            return Task.FromResult(list);
        }

        public Task<Item> GetByIdAsync(int id)
        {
            var it = _db.Items.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(it);
        }

        public Task<Item> CreateAsync(Item item)
        {
            item.Id = (_db.Items.LastOrDefault()?.Id ?? 0) + 1;
            item.ItemCode = GenerateItemCode(item);
            _db.Items.Add(item);
            return Task.FromResult(item);
        }

        public Task UpdateAsync(Item item)
        {
            var existing = _db.Items.FirstOrDefault(i => i.Id == item.Id);
            if (existing != null)
            {
                existing.Name = item.Name;
                existing.Category = item.Category;
                existing.Size = item.Size;
                existing.Color = item.Color;
                existing.IsEnabled = item.IsEnabled;
                existing.StockQty = item.StockQty;
                existing.UnitPrice = item.UnitPrice;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var item = _db.Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.IsEnabled = false;
            }
            return Task.CompletedTask;
        }

        private string GenerateItemCode(Item item)
        {
            var prefix = (item.Category ?? "ITEM").ToUpper().Replace(" ", "");
            var rnd = new Random();
            return $"{prefix}-{rnd.Next(1000, 9999)}";
        }
    }
}