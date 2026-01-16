using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public interface IItemService
    {
        Task<List<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(int id);
        Task<Item> CreateAsync(Item item);
        Task UpdateAsync(Item item);
        Task DeleteAsync(int id);
    }
}