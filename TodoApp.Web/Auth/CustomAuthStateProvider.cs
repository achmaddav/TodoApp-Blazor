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
            // Gunakan cached user jika sudah ada
            if (_currentUser.Identity?.IsAuthenticated == true)
                return new AuthenticationState(_currentUser);

            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            var principal = GetClaimsFromToken(token);
            if (principal is null)
                return _anonymous;

            _currentUser = principal;
            return new AuthenticationState(_currentUser);
        }
        catch
        {
            return _anonymous;
        }
    }

    public async Task MarkAsAuthenticatedAsync(string token)
    {
        try
        {
            await _localStorage.SetItemAsStringAsync("authToken", token);

            var principal = GetClaimsFromToken(token);
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
        }
        catch { }

        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(_anonymous));
    }

    private ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;

            var jwt = handler.ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow) return null;

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return null;
        }
    }
}