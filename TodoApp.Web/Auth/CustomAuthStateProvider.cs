using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace TodoApp.Web.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    private readonly AuthenticationState _anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public CustomAuthStateProvider(ILocalStorageService localStorage)
        => _localStorage = localStorage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Selalu baca dari localStorage, jangan cache
            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                return _anonymous;
            }

            var principal = ParseToken(token);
            if (principal is null)
            {
                // Token invalid atau expired, hapus dari storage
                await _localStorage.RemoveItemAsync("authToken");
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                return _anonymous;
            }

            _currentUser = principal;
            return new AuthenticationState(_currentUser);
        }
        catch
        {
            // localStorage belum tersedia (pre-render)
            return _anonymous;
        }
    }

    public async Task MarkAsAuthenticatedAsync(string token)
    {
        try
        {
            await _localStorage.SetItemAsStringAsync("authToken", token);

            var principal = ParseToken(token);
            if (principal is null) return;

            _currentUser = principal;
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_currentUser)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MarkAsAuthenticated Error: {ex.Message}");
        }
    }

    public async Task MarkAsLoggedOutAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userInfo");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout Error: {ex.Message}");
        }
        finally
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }
    }

    private static ClaimsPrincipal? ParseToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;

            var jwt = handler.ReadJwtToken(token);

            if (jwt.ValidTo.ToUniversalTime() < DateTime.UtcNow)
            {
                Console.WriteLine("Token expired.");
                return null;
            }

            // JWT mengubah ClaimTypes.Email menjadi nama panjang
            // Kita mapping ulang ke nama yang pendek
            var claims = jwt.Claims.Select(c => c.Type switch
            {
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                    => new Claim(ClaimTypes.NameIdentifier, c.Value),
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
                    => new Claim(ClaimTypes.Email, c.Value),
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
                    => new Claim(ClaimTypes.Name, c.Value),
                _ => c
            }).ToList();

            var identity = new ClaimsIdentity(claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ParseToken Error: {ex.Message}");
            return null;
        }
    }
}