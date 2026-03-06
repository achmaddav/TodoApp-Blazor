using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWork uow, IConfiguration config)
        {
            _uow = uow;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto,
            CancellationToken ct = default)
        {
            var exists = await _uow.Users.ExistsAsync(u => u.Email == dto.Email.ToLower(), ct);
            if (exists)
                throw new InvalidOperationException("Email already registered.");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User(dto.FullName, dto.Email, hash);

            await _uow.Users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto,
            CancellationToken ct = default)
        {
            var user = await _uow.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower(), ct)
                ?? throw new UnauthorizedAccessException("Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            user.RecordLogin();
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync(ct);

            return GenerateAuthResponse(user);
        }

        public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken,
            CancellationToken ct = default)
            => throw new NotImplementedException("Refresh token coming soon.");

        public Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
            => throw new NotImplementedException("Revoke token coming soon.");

        // ─── Private Helpers ──────────────────────────────────────────

        private AuthResponseDto GenerateAuthResponse(User user)
        {
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddHours(
                double.Parse(_config["Jwt:ExpiryHours"] ?? "24"));

            return new AuthResponseDto(
                Token: token,
                RefreshToken: refreshToken,
                ExpiresAt: expiresAt,
                User: new UserDto(user.Id, user.FullName, user.Email, user.LastLoginAt)
            );
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    double.Parse(_config["Jwt:ExpiryHours"] ?? "24")),
                signingCredentials: new SigningCredentials(key,
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
