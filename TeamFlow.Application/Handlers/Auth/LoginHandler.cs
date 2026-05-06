using MediatR;
using TeamFlow.Application.Commands.Auth;
using TeamFlow.Application.DTOs;
using TeamFlow.Application.Interfaces;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Exceptions;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Application.Handlers.Auth;

public class LoginHandler : IRequestHandler<LoginCommand, AuthDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == request.Email)
            ?? throw new NotFoundException(nameof(User), request.Email);

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new ForbiddenException("Invalid email or password.");

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