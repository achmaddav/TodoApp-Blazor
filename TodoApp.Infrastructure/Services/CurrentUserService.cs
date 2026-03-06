using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TodoApp.Application.Interfaces;

namespace TodoApp.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public string Email
            => _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        public bool IsAuthenticated
            => _httpContextAccessor.HttpContext?
                .User.Identity?.IsAuthenticated ?? false;
    }
}
