using System.Threading.Tasks;
using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public interface IAuthService
    {
        Task<CurrentUser?> LoginAsync(string username, string password);
        Task LogoutAsync();
        CurrentUser? GetCurrentUser();
        Task<CurrentUser?> GetUserAsync();
    }
}
