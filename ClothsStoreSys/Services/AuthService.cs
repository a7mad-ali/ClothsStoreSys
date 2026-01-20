using System.Threading.Tasks;
using ClothsStoreSys.Models;
using ClothsStoreSys.Data;

namespace ClothsStoreSys.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage.ProtectedLocalStorage _localStorage;
        private CurrentUser? _currentUser;

        public AuthService(AppDbContext db, Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage.ProtectedLocalStorage localStorage) 
        { 
            _db = db; 
            _localStorage = localStorage;
        }

        public async Task<CurrentUser?> LoginAsync(string username, string password)
        {
            var u = _db.Users.FirstOrDefault(x => x.Username == username && x.PasswordHash == password);
            if (u == null) return null;

            _currentUser = new CurrentUser { Id = u.Id, Username = u.Username, Role = u.Role };
            await _localStorage.SetAsync("user_session", _currentUser);
            return _currentUser;
        }

        public async Task LogoutAsync()
        {
            _currentUser = null;
            await _localStorage.DeleteAsync("user_session");
        }

        public CurrentUser? GetCurrentUser() => _currentUser;

        public async Task<CurrentUser?> GetUserAsync()
        {
            if (_currentUser != null) return _currentUser;

            try
            {
                var result = await _localStorage.GetAsync<CurrentUser>("user_session");
                if (result.Success)
                {
                    _currentUser = result.Value;
                }
            }
            catch
            {
                // JavaScript interop calls cannot be issued at this time (e.g. during prerendering)
            }

            return _currentUser;
        }
    }
}
