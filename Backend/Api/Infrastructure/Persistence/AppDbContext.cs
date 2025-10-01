using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SportActivity> Activities => Set<SportActivity>();
        public DbSet<ActivityOccurrence> ActivityOccurrences => Set<ActivityOccurrence>();
        public DbSet<Place> Places => Set<Place>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            foreach (var et in b.Model.GetEntityTypes()
                         .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
            {
                b.Entity(et.ClrType)
                    .Property<Guid>(nameof(BaseEntity.Id))
                    .ValueGeneratedNever();

                b.Entity(et.ClrType)
                    .Property<byte[]>(nameof(BaseEntity.RowVersion))
                    .IsRowVersion();

                b.Entity(et.ClrType)
                    .Property<DateTime>(nameof(BaseEntity.CreatedAtUtc))
                    .IsRequired();

                b.Entity(et.ClrType)
                    .Property<DateTime>(nameof(BaseEntity.UpdatedAtUtc))
                    .IsRequired();
            }

            b.Entity<SportActivity>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(120).IsRequired();
                e.Property(x => x.Description).HasMaxLength(2000);
                e.Property(x => x.Price).HasColumnType("decimal(10,2)");

                e.HasIndex(x => new { x.IsActive, x.Environment });

                e.HasOne(x => x.Category)
                    .WithMany(c => c.Activities)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            b.Entity<Category>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Description).HasMaxLength(1000);
                e.HasIndex(x => new { x.Name, x.IsActive });
            });

            b.Entity<Place>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(120).IsRequired();
                e.Property(x => x.Address).HasMaxLength(200);

                e.HasIndex(x => new { x.IsActive, x.Environment });
                e.HasIndex(x => new { x.Latitude, x.Longitude });
            });

            b.Entity<ActivityOccurrence>(e =>
            {
                e.Property(x => x.PriceOverride).HasColumnType("decimal(10,2)");

                e.HasOne(x => x.Activity)
                    .WithMany(a => a.Occurrences)
                    .HasForeignKey(x => x.ActivityId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Place)
                    .WithMany(p => p.Occurrences)
                    .HasForeignKey(x => x.PlaceId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ignorera beräknad property
                e.Ignore(x => x.EffectiveCapacity);

                e.HasIndex(x => new { x.StartUtc, x.EndUtc });
                e.HasIndex(x => new { x.PlaceId, x.StartUtc });
                e.HasIndex(x => new { x.ActivityId, x.StartUtc });
            });

            b.Entity<Booking>(e =>
            {
                e.HasOne(x => x.ActivityOccurrence)
                    .WithMany(o => o.Bookings)
                    .HasForeignKey(x => x.ActivityOccurrenceId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.ActivityOccurrenceId, x.UserId }).IsUnique();
                e.HasIndex(x => new { x.UserId, x.Status });
            });

            b.Entity<User>(e =>
            {
                e.Property(x => x.Firstname).HasMaxLength(100);
                e.Property(x => x.Lastname).HasMaxLength(100);
            });
            
            b.Entity<RefreshToken>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.TokenHash).IsRequired().HasMaxLength(200);
                e.Property(x => x.UserId).IsRequired();

                e.HasOne(x => x.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.UserId, x.TokenHash }).IsUnique();
                e.HasIndex(x => x.ExpiresAtUtc);
            });
        }

        // ===== Automatiska tidsstämplar =====
        public override int SaveChanges()
        {
            TouchTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            TouchTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void TouchTimestamps()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.Id == Guid.Empty)
                        entry.Entity.Id = Guid.NewGuid();

                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.UpdatedAtUtc = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = now;
                }
            }
        }
    }
}