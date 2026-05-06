using MediatR;
using TeamFlow.Application.Commands.Auth;
using TeamFlow.Application.DTOs;
using TeamFlow.Application.Interfaces;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Exceptions;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Application.Handlers.Auth;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUsers = await _unitOfWork.Users.GetAllAsync();
        if (existingUsers.Any(u => u.Email == request.Email))
            throw new ConflictException($"Email '{request.Email}' is already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);

        var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthDto
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = refreshToken.Token,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt
            }
        };
    }
}