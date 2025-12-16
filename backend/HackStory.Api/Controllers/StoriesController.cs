using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HackStory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoriesController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<StoriesController> _logger;

    public StoriesController(
        IMemoryCache cache,
        ILogger<StoriesController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetStories()
    {
        // キャッシュから取得（1時間キャッシュ）
        const string cacheKey = "stories_list";
        if (_cache.TryGetValue(cacheKey, out var cachedStories))
        {
            return Ok(cachedStories);
        }

        // メタデータのみ返却（実際のコンテンツはフロントエンドで静的JSONから読み込む）
        var stories = new[]
        {
            new { Id = "story-1", Title = "企業セキュリティの脅威", Description = "企業ネットワークに侵入の兆候を発見します。" }
        };

        _cache.Set(cacheKey, stories, TimeSpan.FromHours(1));
        return Ok(stories);
    }

    [HttpGet("{id}")]
    public IActionResult GetStory(string id)
    {
        // メタデータのみ返却
        var story = new
        {
            Id = id,
            Title = "企業セキュリティの脅威",
            Description = "企業ネットワークに侵入の兆候を発見します。",
            ContentPath = $"/stories/{id}.json"
        };

        return Ok(story);
    }
}

