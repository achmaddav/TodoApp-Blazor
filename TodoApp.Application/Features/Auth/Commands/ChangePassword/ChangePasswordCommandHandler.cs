using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public ChangePasswordCommandHandler(
            IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(
            ChangePasswordCommand request, CancellationToken ct)
        {
            var user = await _uow.Users.GetByIdAsync(_currentUser.UserId, ct);
            if (user is null)
                return Result.Failure("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return Result.Failure("Current password is incorrect.");

            // Ganti assignment langsung dengan method
            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatePassword(newHash);  // ← pakai method

            await _uow.Users.UpdateAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
