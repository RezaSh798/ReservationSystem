using Microsoft.EntityFrameworkCore;
using ReservationService.Domain.Entities;

namespace ReservationService.Infrastructure.Persistence;

public class ReservationDbContext : DbContext
{
    public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<ReservationEntity> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReservationEntity>().HasKey(r => r.Id);
        base.OnModelCreating(modelBuilder);
    }
}