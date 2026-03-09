using MediatR;
using TodoApp.Application.Common.Models;

namespace TodoApp.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword) : IRequest<Result>;
}
