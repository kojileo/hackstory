using HackStory.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackStory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoriesController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly ILogger<StoriesController> _logger;

    public StoriesController(
        IStoryService storyService,
        ILogger<StoriesController> logger)
    {
        _storyService = storyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStories()
    {
        try
        {
            var stories = await _storyService.GetAllStoriesAsync();
            
            // メタデータのみ返却（実際のコンテンツはフロントエンドで静的JSONから読み込む）
            var storyList = stories.Select(s => new
            {
                Id = s.Id.ToString(),
                Title = s.Title,
                Description = s.Description,
                ContentPath = $"/stories/story-{s.Id.ToString().Substring(0, 8)}.json"
            });

            return Ok(storyList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stories");
            
            // データベース接続エラーの場合、デフォルトのストーリーリストを返す
            var defaultStories = new[]
            {
                new
                {
                    Id = "story-1",
                    Title = "企業セキュリティの脅威",
                    Description = "企業ネットワークに侵入の兆候を発見します。",
                    ContentPath = "/stories/story-1.json"
                }
            };
            
            return Ok(defaultStories);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStory(string id)
    {
        try
        {
            // 文字列IDをGuidに変換
            var storyGuid = ProgressControllerHelpers.ConvertStoryIdToGuid(id);
            var story = await _storyService.GetStoryByIdAsync(storyGuid);

            if (story == null)
            {
                // データベースにない場合は、メタデータのみ返却
                var storyDto = new
                {
                    Id = id,
                    Title = "企業セキュリティの脅威",
                    Description = "企業ネットワークに侵入の兆候を発見します。",
                    ContentPath = $"/stories/{id}.json"
                };
                return Ok(storyDto);
            }

            // メタデータのみ返却
            var storyDto2 = new
            {
                Id = story.Id.ToString(),
                Title = story.Title,
                Description = story.Description,
                ContentPath = $"/stories/story-{story.Id.ToString().Substring(0, 8)}.json"
            };

            return Ok(storyDto2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting story");
            return StatusCode(500, new { error = "Failed to get story" });
        }
    }
}

