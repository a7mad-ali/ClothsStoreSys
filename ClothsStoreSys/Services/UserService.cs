using ClothsStoreSys.Data;
using ClothsStoreSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Add this using directive

namespace ClothsStoreSys.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db) { _db = db; }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            // NOTE: plain-text comparison for seed. Replace with real hashing in production.
            return await Task.Run(() =>
                _db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password)
            );
        }
    }
}