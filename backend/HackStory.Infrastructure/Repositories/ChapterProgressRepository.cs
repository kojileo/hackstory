using HackStory.Application.Interfaces;
using HackStory.Domain.Entities;
using HackStory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HackStory.Infrastructure.Repositories;

public class ChapterProgressRepository : Repository<ChapterProgress>, IChapterProgressRepository
{
    public ChapterProgressRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ChapterProgress?> GetByUserAndStoryAndChapterAsync(Guid userId, Guid storyId, int chapterId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(cp => 
                cp.UserId == userId && 
                cp.StoryId == storyId && 
                cp.ChapterId == chapterId);
    }

    public async Task<IEnumerable<ChapterProgress>> GetByUserAndStoryAsync(Guid userId, Guid storyId)
    {
        return await _dbSet
            .Where(cp => cp.UserId == userId && cp.StoryId == storyId)
            .OrderBy(cp => cp.ChapterId)
            .ToListAsync();
    }
}

