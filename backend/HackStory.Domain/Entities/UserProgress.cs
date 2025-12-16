namespace HackStory.Domain.Entities;

public class UserProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int TotalChaptersCompleted { get; set; } = 0;
    public int TotalChallengesCompleted { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
}

