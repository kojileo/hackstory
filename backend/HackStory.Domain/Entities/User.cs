namespace HackStory.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? FirebaseUid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<ChapterProgress> ChapterProgresses { get; set; } = new List<ChapterProgress>();
    public ICollection<ChallengeProgress> ChallengeProgresses { get; set; } = new List<ChallengeProgress>();
    public UserProgress? UserProgress { get; set; }
}

