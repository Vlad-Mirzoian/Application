using EventApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Participant> Participants { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(u => u.CreatedEvents)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasKey(ep => new { ep.EventId, ep.UserId });

            modelBuilder.Entity<Participant>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Event>().HasIndex(e => e.StartDateTime);

            var user1 = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Email = "user1@example.com",
                PasswordHash = "$2a$12$yutgWM8YE0kHJ7t4EF4tz.qiDbwj3DlYSRKfL3T59RZ/0y4KPTidy",
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            var user2 = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = "user2@example.com",
                PasswordHash = "$2a$12$23Tr52d2PMi5.o1tt4nld.tP0O5.LkpgN9jZMakBTp5vt9HiyPH/m",
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            modelBuilder.Entity<User>().HasData(user1, user2);

            var event1 = new Event
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Public Event 1",
                StartDateTime = new DateTime(2025, 10, 20, 12, 0, 0, DateTimeKind.Utc),
                Location = "City Center",
                Capacity = 10,
                Visibility = true,
                CreatorId = user1.Id,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            var event2 = new Event
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "Public Event 2",
                StartDateTime = new DateTime(2025, 10, 21, 12, 0, 0, DateTimeKind.Utc),
                Location = "Park",
                Capacity = 15,
                Visibility = true,
                CreatorId = user1.Id,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            var event3 = new Event
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Title = "Private Event",
                StartDateTime = new DateTime(2025, 10, 22, 12, 0, 0, DateTimeKind.Utc),
                Location = "Home",
                Capacity = 5,
                Visibility = false,
                CreatorId = user2.Id,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            modelBuilder.Entity<Event>().HasData(event1, event2, event3);

            modelBuilder.Entity<Participant>().HasData(
                            new Participant
                            {
                                EventId = event1.Id,
                                UserId = user2.Id,
                                JoinedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)
                            }
                        );
        }
    }
}