using MediatR;
using TeamFlow.Application.DTOs;

namespace TeamFlow.Application.Commands.Auth;

public record UpdateProfileCommand(
    Guid UserId,
    string FullName,
    string? AvatarUrl) : IRequest<UserDto>;
