using System.Threading.Tasks;
using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    // Simple scoped session-like auth service
    public class AuthService
    {
        private User? _currentUser;

        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public Task SignInAsync(User user)
        {
            _currentUser = user;
            return Task.CompletedTask;
        }

        public Task SignOutAsync()
        {
            _currentUser = null;
            return Task.CompletedTask;
        }
    }
}
