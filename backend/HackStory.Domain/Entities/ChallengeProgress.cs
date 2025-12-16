namespace HackStory.Domain.Entities;

public class ChallengeProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ChallengeId { get; set; }
    public bool Completed { get; set; } = false;
    public int Attempts { get; set; } = 0;
    public DateTime? CompletedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Challenge Challenge { get; set; } = null!;
}

