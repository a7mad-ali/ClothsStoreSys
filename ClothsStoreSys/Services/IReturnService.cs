using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public interface IReturnService
    {
        Task<Return> ProcessReturnAsync(Return ret, List<ReturnItem> items);
    }
}