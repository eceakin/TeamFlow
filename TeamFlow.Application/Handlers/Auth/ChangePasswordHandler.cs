using MediatR;
using TeamFlow.Application.Commands.Auth;
using TeamFlow.Application.Interfaces;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Exceptions;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Application.Handlers.Auth;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new ForbiddenException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}