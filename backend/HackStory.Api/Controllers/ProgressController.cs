using HackStory.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HackStory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IChapterProgressService _chapterProgressService;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(
        IChapterProgressService chapterProgressService,
        ILogger<ProgressController> logger)
    {
        _chapterProgressService = chapterProgressService;
        _logger = logger;
    }

    [HttpPost("stories/{storyId}/chapters/{chapterId}")]
    public async Task<IActionResult> SaveChapterProgress(
        string storyId,
        int chapterId,
        [FromBody] SaveProgressRequest request)
    {
        try
        {
            // TODO: 認証からユーザーIDを取得（現在は仮のID）
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            
            // ストーリーIDをGuidに変換（story-1形式から）
            var storyGuid = ProgressControllerHelpers.ConvertStoryIdToGuid(storyId);

            var progress = await _chapterProgressService.SaveProgressAsync(
                userId,
                storyGuid,
                chapterId,
                request.Completed,
                request.Choices != null ? JsonDocument.Parse(JsonSerializer.Serialize(request.Choices)) : null);

            return Ok(new { success = true, progress });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving chapter progress");
            return StatusCode(500, new { error = "Failed to save progress" });
        }
    }

    [HttpGet("stories/{storyId}/chapters/{chapterId}")]
    public async Task<IActionResult> GetChapterProgress(
        string storyId,
        int chapterId)
    {
        try
        {
            // TODO: 認証からユーザーIDを取得（現在は仮のID）
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            
            var storyGuid = ProgressControllerHelpers.ConvertStoryIdToGuid(storyId);

            var progress = await _chapterProgressService.GetChapterProgressAsync(userId, storyGuid, chapterId);

            if (progress == null)
            {
                return NotFound();
            }

            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chapter progress");
            return StatusCode(500, new { error = "Failed to get progress" });
        }
    }

    [HttpGet("stories/{storyId}")]
    public async Task<IActionResult> GetStoryProgress(string storyId)
    {
        try
        {
            // TODO: 認証からユーザーIDを取得（現在は仮のID）
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            
            var storyGuid = ProgressControllerHelpers.ConvertStoryIdToGuid(storyId);

            var progress = await _chapterProgressService.GetUserStoryProgressAsync(userId, storyGuid);

            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting story progress");
            return StatusCode(500, new { error = "Failed to get progress" });
        }
    }
}

public class SaveProgressRequest
{
    public bool Completed { get; set; }
    public Dictionary<string, object>? Choices { get; set; }
}

// ヘルパーメソッド
public static class ProgressControllerHelpers
{
    public static Guid ConvertStoryIdToGuid(string storyId)
    {
        // story-1形式をGuidに変換（簡易実装）
        // 実際の実装では、データベースからストーリーIDを取得する必要がある
        if (Guid.TryParse(storyId, out var guid))
        {
            return guid;
        }
        
        // story-1形式の場合、固定のGuidを返す（開発用）
        return storyId switch
        {
            "story-1" => Guid.Parse("11111111-1111-1111-1111-111111111111"),
            _ => Guid.NewGuid()
        };
    }
}

