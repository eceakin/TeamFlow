using MediatR;
using TeamFlow.Application.Commands.Auth;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Exceptions;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Application.Handlers.Auth;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _unitOfWork.RefreshTokens.GetAllAsync();
        var refreshToken = tokens.FirstOrDefault(t => t.Token == request.RefreshToken)
            ?? throw new NotFoundException(nameof(RefreshToken), request.RefreshToken);

        refreshToken.IsRevoked = true;
        _unitOfWork.RefreshTokens.Update(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}