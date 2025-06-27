using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entity;

namespace Notification.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<NotificationEntity> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationEntity>().HasKey(n => n.Id);
        base.OnModelCreating(modelBuilder);
    }
}