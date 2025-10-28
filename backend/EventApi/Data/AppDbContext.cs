using EventApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<EventTag> EventTags { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasIndex(e => e.StartDateTime);
                entity.HasIndex(e => e.Title).IsUnique();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Location).IsRequired();
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(ep => new { ep.EventId, ep.UserId });

                entity.HasOne(ep => ep.Event)
                      .WithMany(e => e.Participants)
                      .HasForeignKey(ep => ep.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ep => ep.User)
                      .WithMany(u => u.Participations)
                      .HasForeignKey(ep => ep.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(t => t.Name).IsRequired();

                entity.HasIndex(t => t.Name)
                      .IsUnique()
                      .HasFilter("LOWER(\"Name\") IS NOT NULL");
            });

            modelBuilder.Entity<EventTag>(entity =>
            {
                entity.HasKey(et => new { et.EventId, et.TagId });

                entity.HasOne(et => et.Event)
                      .WithMany(e => e.EventTags)
                      .HasForeignKey(et => et.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(et => et.Tag)
                      .WithMany(t => t.EventTags)
                      .HasForeignKey(et => et.TagId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(u => u.CreatedEvents)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
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
                StartDateTime = new DateTime(2025, 11, 20, 12, 0, 0, DateTimeKind.Utc),
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
                StartDateTime = new DateTime(2025, 11, 21, 12, 0, 0, DateTimeKind.Utc),
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
                StartDateTime = new DateTime(2025, 11, 22, 12, 0, 0, DateTimeKind.Utc),
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

            var tagTech = new Tag { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Technology" };
            var tagArt = new Tag { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Art" };
            var tagBiz = new Tag { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Business" };
            var tagMus = new Tag { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Name = "Music" };

            modelBuilder.Entity<Tag>().HasData(tagTech, tagArt, tagBiz, tagMus);

            modelBuilder.Entity<EventTag>().HasData(
                new EventTag { EventId = event1.Id, TagId = tagTech.Id },
                new EventTag { EventId = event1.Id, TagId = tagBiz.Id },
                new EventTag { EventId = event2.Id, TagId = tagArt.Id }
            );
        }
    }
}