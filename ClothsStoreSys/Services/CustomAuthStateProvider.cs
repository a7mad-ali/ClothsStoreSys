using System.Security.Claims;
using System.Threading.Tasks;
using ClothsStoreSys.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClothsStoreSys.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;

        public CustomAuthStateProvider(IAuthService authService) => _authService = authService;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _authService.GetCurrentUser();
            ClaimsIdentity identity;
            if (user == null)
            {
                identity = new ClaimsIdentity();
            }
            else
            {
                identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }, "Custom");
            }

            var principal = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(principal));
        }

        public void NotifyAuthChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
