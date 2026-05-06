using MediatR;
using TeamFlow.Application.Commands.Auth;
using TeamFlow.Application.DTOs;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Exceptions;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Application.Handlers.Auth;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        user.FullName = request.FullName;
        user.AvatarUrl = request.AvatarUrl;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        };
    }
 }

