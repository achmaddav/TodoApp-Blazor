using TodoApp.Application.DTOs;

namespace TodoApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
    }
}
