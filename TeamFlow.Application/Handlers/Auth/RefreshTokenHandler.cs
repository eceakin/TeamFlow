using MediatR;
using TeamFlow.Application.Commands.Auth;
using TeamFlow.Application.DTOs;
using TeamFlow.Application.Interfaces;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Exceptions;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Application.Handlers.Auth;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RefreshTokenHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _unitOfWork.RefreshTokens.GetAllAsync();
        var existingToken = tokens.FirstOrDefault(t => t.Token == request.RefreshToken)
            ?? throw new NotFoundException(nameof(RefreshToken), request.RefreshToken);

        if (existingToken.IsRevoked || existingToken.ExpiresAt < DateTime.UtcNow)
            throw new ForbiddenException("Refresh token is invalid or expired.");

        existingToken.IsRevoked = true;
        _unitOfWork.RefreshTokens.Update(existingToken);

        var users = await _unitOfWork.Users.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Id == existingToken.UserId)
            ?? throw new NotFoundException(nameof(User), existingToken.UserId);

        var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id);
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthDto
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = newRefreshToken.Token,
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