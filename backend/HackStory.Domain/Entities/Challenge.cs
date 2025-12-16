using System.Text.Json;

namespace HackStory.Domain.Entities;

public class Challenge
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Difficulty { get; set; } // 1-5
    public string Solution { get; set; } = string.Empty;
    public JsonDocument? Hints { get; set; } // ヒントの配列
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<ChallengeProgress> ChallengeProgresses { get; set; } = new List<ChallengeProgress>();
}

