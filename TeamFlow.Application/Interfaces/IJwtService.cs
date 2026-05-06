using TeamFlow.Domain.Entities;

namespace TeamFlow.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
}

