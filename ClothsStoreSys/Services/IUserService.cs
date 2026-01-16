using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
    }
}