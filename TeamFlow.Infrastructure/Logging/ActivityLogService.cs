using TeamFlow.Application.Interfaces;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Infrastructure.Logging;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityLogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(Guid projectId, Guid userId, string action, string entityType,
        Guid entityId, string? oldValue = null, string? newValue = null)
    {
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldValue,
            NewValue = newValue,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ActivityLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }
}