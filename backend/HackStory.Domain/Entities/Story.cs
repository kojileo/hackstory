namespace HackStory.Domain.Entities;

public class Story
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ContentPath { get; set; } = string.Empty; // JSONファイルのパス
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<ChapterProgress> ChapterProgresses { get; set; } = new List<ChapterProgress>();
}

