using MediatR;

namespace TeamFlow.Application.Commands.Auth;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;