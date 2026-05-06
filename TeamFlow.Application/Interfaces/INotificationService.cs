using TeamFlow.Domain.Enums;

namespace TeamFlow.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string message, NotificationType type, Guid? relatedEntityId = null);
}

