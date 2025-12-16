using HackStory.Domain.Entities;

namespace HackStory.Application.Interfaces;

public interface IChapterProgressRepository : IRepository<ChapterProgress>
{
    Task<ChapterProgress?> GetByUserAndStoryAndChapterAsync(Guid userId, Guid storyId, int chapterId);
    Task<IEnumerable<ChapterProgress>> GetByUserAndStoryAsync(Guid userId, Guid storyId);
}

