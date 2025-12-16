using HackStory.Application.Interfaces;
using HackStory.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HackStory.Application.Services;

public class StoryService : IStoryService
{
    private readonly IRepository<Story> _storyRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<StoryService> _logger;

    public StoryService(
        IRepository<Story> storyRepository,
        IMemoryCache cache,
        ILogger<StoryService> logger)
    {
        _storyRepository = storyRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Story>> GetAllStoriesAsync()
    {
        const string cacheKey = "stories_all";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Story>? cachedStories))
        {
            return cachedStories ?? Enumerable.Empty<Story>();
        }

        try
        {
            var stories = await _storyRepository.GetAllAsync();
            
            // 1時間キャッシュ
            _cache.Set(cacheKey, stories, TimeSpan.FromHours(1));
            
            return stories;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get stories from database, returning empty list");
            // データベースエラーの場合、空のリストを返す
            return Enumerable.Empty<Story>();
        }
    }

    public async Task<Story?> GetStoryByIdAsync(Guid id)
    {
        var cacheKey = $"story_{id}";
        
        if (_cache.TryGetValue(cacheKey, out Story? cachedStory))
        {
            return cachedStory;
        }

        var story = await _storyRepository.GetByIdAsync(id);
        
        if (story != null)
        {
            // 1時間キャッシュ
            _cache.Set(cacheKey, story, TimeSpan.FromHours(1));
        }
        
        return story;
    }
}

