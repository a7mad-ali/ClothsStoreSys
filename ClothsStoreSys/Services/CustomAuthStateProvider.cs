using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClothsStoreSys.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _auth;

        public CustomAuthStateProvider(AuthService auth) => _auth = auth;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity;
            if (_auth.CurrentUser is null)
            {
                identity = new ClaimsIdentity();
            }
            else
            {
                identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, _auth.CurrentUser.Username),
                    new Claim(ClaimTypes.Role, _auth.CurrentUser.Role ?? "Cashier")
                }, "Custom");
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public void NotifyAuthenticationStateChanged()
        {
            var task = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(task);
        }
    }
}
