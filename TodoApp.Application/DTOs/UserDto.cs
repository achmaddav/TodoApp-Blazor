namespace TodoApp.Application.DTOs
{
    public record UserDto(
        Guid Id,
        string FullName,
        string Email,
        DateTime? LastLoginAt
    );

    public record RegisterDto(
        string FullName,
        string Email,
        string Password,
        string ConfirmPassword
    );

    public record LoginDto(
        string Email,
        string Password
    );

    public record AuthResponseDto(
        string Token,
        string RefreshToken,
        DateTime ExpiresAt,
        UserDto User
    );
}
