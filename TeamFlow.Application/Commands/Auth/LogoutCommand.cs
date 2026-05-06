using MediatR;

namespace TeamFlow.Application.Commands.Auth;

public record LogoutCommand(
    string RefreshToken) : IRequest;
