using MediatR;
using TeamFlow.Application.DTOs;

namespace TeamFlow.Application.Commands.Auth;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password) : IRequest<AuthDto>;
