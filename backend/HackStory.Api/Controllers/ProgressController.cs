using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace HackStory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(
        IMemoryCache cache,
        ILogger<ProgressController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [HttpPost("stories/{storyId}/chapters/{chapterId}")]
    public IActionResult SaveChapterProgress(
        string storyId,
        int chapterId,
        [FromBody] JsonDocument progressData)
    {
        // TODO: データベースに保存
        // 現在はキャッシュのみ（デバウンス処理のため）
        
        var cacheKey = $"progress_{storyId}_{chapterId}";
        _cache.Set(cacheKey, progressData, TimeSpan.FromMinutes(5));

        return Ok(new { success = true });
    }

    [HttpGet("users/{userId}")]
    public IActionResult GetUserProgress(string userId)
    {
        // キャッシュから取得（5分キャッシュ）
        const string cacheKey = "user_progress";
        if (_cache.TryGetValue(cacheKey, out var cachedProgress))
        {
            return Ok(cachedProgress);
        }

        // TODO: データベースから取得
        var progress = new
        {
            userId,
            level = 1,
            experience = 0,
            totalChaptersCompleted = 0,
            totalChallengesCompleted = 0
        };

        _cache.Set(cacheKey, progress, TimeSpan.FromMinutes(5));
        return Ok(progress);
    }
}

