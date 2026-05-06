using TeamFlow.Domain.Entities;

namespace TeamFlow.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Project> Projects { get; }
    IRepository<ProjectTask> Tasks { get; }
    IRepository<ProjectMember> ProjectMembers { get; }
    IRepository<Sprint> Sprints { get; }
    IRepository<Comment> Comments { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<ActivityLog> ActivityLogs { get; }
    IRepository<Attachment> Attachments { get; }
    IRepository<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}