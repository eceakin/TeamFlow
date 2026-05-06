using MediatR;
using TeamFlow.Application.DTOs;

namespace TeamFlow.Application.Commands.Auth;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthDto>;
