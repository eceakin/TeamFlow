namespace TeamFlow.Application.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(Guid projectId, Guid userId, string action, string entityType, Guid entityId,
        string? oldValue = null, string? newValue = null);
}

