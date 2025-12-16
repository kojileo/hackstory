using HackStory.Domain.Entities;
using System.Text.Json;

namespace HackStory.Application.Services;

public interface IChapterProgressService
{
    Task<ChapterProgress> SaveProgressAsync(
        Guid userId,
        Guid storyId,
        int chapterId,
        bool completed,
        JsonDocument? choices);
    
    Task<IEnumerable<ChapterProgress>> GetUserStoryProgressAsync(Guid userId, Guid storyId);
    Task<ChapterProgress?> GetChapterProgressAsync(Guid userId, Guid storyId, int chapterId);
}

