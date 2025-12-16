namespace HackStory.Domain.Entities;

public class ChapterProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StoryId { get; set; }
    public int ChapterId { get; set; }
    public bool Completed { get; set; } = false;
    public string? ChoicesJson { get; set; } // 選択した選択肢の記録（JSON文字列）
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Story Story { get; set; } = null!;
}

