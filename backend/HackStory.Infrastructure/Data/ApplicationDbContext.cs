using HackStory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HackStory.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<ChapterProgress> ChapterProgresses { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<ChallengeProgress> ChallengeProgresses { get; set; }
    public DbSet<UserProgress> UserProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.FirebaseUid).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.FirebaseUid).HasMaxLength(128);
        });

        // Story configuration
        modelBuilder.Entity<Story>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ContentPath).IsRequired().HasMaxLength(500);
        });

        // ChapterProgress configuration
        modelBuilder.Entity<ChapterProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.StoryId, e.ChapterId }).IsUnique();
            entity.Property(e => e.ChoicesJson).HasColumnType("jsonb"); // JSONB型として保存
            entity.HasOne(e => e.User)
                .WithMany(u => u.ChapterProgresses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Story)
                .WithMany(s => s.ChapterProgresses)
                .HasForeignKey(e => e.StoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Challenge configuration
        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Solution).IsRequired();
            entity.Property(e => e.Difficulty).IsRequired();
            entity.Property(e => e.HintsJson).HasColumnType("jsonb"); // JSONB型として保存
        });

        // ChallengeProgress configuration
        modelBuilder.Entity<ChallengeProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ChallengeId }).IsUnique();
            entity.HasOne(e => e.User)
                .WithMany(u => u.ChallengeProgresses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Challenge)
                .WithMany(c => c.ChallengeProgresses)
                .HasForeignKey(e => e.ChallengeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserProgress configuration
        modelBuilder.Entity<UserProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User)
                .WithOne(u => u.UserProgress)
                .HasForeignKey<UserProgress>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

