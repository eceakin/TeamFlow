using MediatR;
using TeamFlow.Application.DTOs;

namespace TeamFlow.Application.Commands.Auth;

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<AuthDto>;
