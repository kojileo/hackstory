using HackStory.Application.Interfaces;
using HackStory.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HackStory.Application.Services;

public class ChapterProgressService : IChapterProgressService
{
    private readonly IChapterProgressRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ChapterProgressService> _logger;

    public ChapterProgressService(
        IChapterProgressRepository repository,
        IMemoryCache cache,
        ILogger<ChapterProgressService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ChapterProgress> SaveProgressAsync(
        Guid userId,
        Guid storyId,
        int chapterId,
        bool completed,
        JsonDocument? choices)
    {
        var existing = await _repository.GetByUserAndStoryAndChapterAsync(userId, storyId, chapterId);

        // JsonDocumentをJSON文字列に変換
        var choicesJson = choices != null ? JsonSerializer.Serialize(choices) : null;

        if (existing != null)
        {
            existing.Completed = completed;
            existing.ChoicesJson = choicesJson;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(existing);

            // キャッシュを無効化
            var cacheKey = $"progress_{userId}_{storyId}_{chapterId}";
            _cache.Remove(cacheKey);

            return existing;
        }
        else
        {
            var progress = new ChapterProgress
            {
                UserId = userId,
                StoryId = storyId,
                ChapterId = chapterId,
                Completed = completed,
                ChoicesJson = choicesJson,
                UpdatedAt = DateTime.UtcNow
            };

            return await _repository.AddAsync(progress);
        }
    }

    public async Task<IEnumerable<ChapterProgress>> GetUserStoryProgressAsync(Guid userId, Guid storyId)
    {
        var cacheKey = $"progress_{userId}_{storyId}_all";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<ChapterProgress>? cachedProgress))
        {
            return cachedProgress ?? Enumerable.Empty<ChapterProgress>();
        }

        var progress = await _repository.GetByUserAndStoryAsync(userId, storyId);
        
        // 5分キャッシュ
        _cache.Set(cacheKey, progress, TimeSpan.FromMinutes(5));
        
        return progress;
    }

    public async Task<ChapterProgress?> GetChapterProgressAsync(Guid userId, Guid storyId, int chapterId)
    {
        var cacheKey = $"progress_{userId}_{storyId}_{chapterId}";
        
        if (_cache.TryGetValue(cacheKey, out ChapterProgress? cachedProgress))
        {
            return cachedProgress;
        }

        var progress = await _repository.GetByUserAndStoryAndChapterAsync(userId, storyId, chapterId);
        
        if (progress != null)
        {
            // 5分キャッシュ
            _cache.Set(cacheKey, progress, TimeSpan.FromMinutes(5));
        }
        
        return progress;
    }
}

