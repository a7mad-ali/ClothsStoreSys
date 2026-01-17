using System.Threading.Tasks;
using ClothsStoreSys.Models;
using ClothsStoreSys.Data;

namespace ClothsStoreSys.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private CurrentUser? _currentUser;

        public AuthService(AppDbContext db) { _db = db; }

        public Task<CurrentUser?> LoginAsync(string username, string password)
        {
            // Note: password is plain-text in seed data. Replace with hashing in production.
            var u = _db.Users.FirstOrDefault(x => x.Username == username && x.PasswordHash == password);
            if (u == null) return Task.FromResult<CurrentUser?>(null);

            _currentUser = new CurrentUser { Id = u.Id, Username = u.Username, Role = u.Role };
            return Task.FromResult(_currentUser);
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            return Task.CompletedTask;
        }

        public CurrentUser? GetCurrentUser() => _currentUser;
    }
}
