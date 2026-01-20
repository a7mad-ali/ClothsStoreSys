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
            // Trim inputs to handle any whitespace issues
            username = username?.Trim() ?? "";
            password = password?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return null;

            // Robust check: Get user by username first (case-insensitive in default SQL, but good to be explicit/safe)
            // We use AsEnumerable or simple query to ensure we get the user.
            var u = _db.Users.FirstOrDefault(x => x.Username.Trim() == username);
            
            // If strictly case sensitive check is needed: 
            // if (u == null || !u.Username.Equals(username, StringComparison.Ordinal)) return null;

            // Password check (Assume Check against plain text for now as per DB logic)
            if (u == null || u.PasswordHash.Trim() != password) return null;

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
