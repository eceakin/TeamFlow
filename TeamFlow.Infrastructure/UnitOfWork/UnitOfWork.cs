using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Interfaces;
using TeamFlow.Infrastructure.Persistence.Data;
using TeamFlow.Infrastructure.Persistence.Repositories;

namespace TeamFlow.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new BaseRepository<User>(context);
        Projects = new BaseRepository<Project>(context);
        Tasks = new BaseRepository<ProjectTask>(context);
        ProjectMembers = new BaseRepository<ProjectMember>(context);
        Sprints = new BaseRepository<Sprint>(context);
        Comments = new BaseRepository<Comment>(context);
        Notifications = new BaseRepository<Notification>(context);
        ActivityLogs = new BaseRepository<ActivityLog>(context);
        Attachments = new BaseRepository<Attachment>(context);
        RefreshTokens = new BaseRepository<RefreshToken>(context);
    }

    public IRepository<User> Users { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<ProjectTask> Tasks { get; }
    public IRepository<ProjectMember> ProjectMembers { get; }
    public IRepository<Sprint> Sprints { get; }
    public IRepository<Comment> Comments { get; }
    public IRepository<Notification> Notifications { get; }
    public IRepository<ActivityLog> ActivityLogs { get; }
    public IRepository<Attachment> Attachments { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() =>
        _context.Dispose();
}