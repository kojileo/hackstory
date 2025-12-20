using HackStory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text.Json;

namespace HackStory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
            var userId = await GetUserIdFromClaimsAsync();
            if (userId == null)
            {
                return Unauthorized(new { error = "Invalid user" });
            }
            
            // ストーリーIDをGuidに変換（story-1形式から）
            var storyGuid = ProgressControllerHelpers.ConvertStoryIdToGuid(storyId);

            var progress = await _chapterProgressService.SaveProgressAsync(
                userId.Value,
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
            var userId = await GetUserIdFromClaimsAsync();
            if (userId == null)
            {
                return Unauthorized(new { error = "Invalid user" });
            }
            
            var storyGuid = ProgressControllerHelpers.ConvertStoryIdToGuid(storyId);

            var progress = await _chapterProgressService.GetChapterProgressAsync(userId.Value, storyGuid, chapterId);

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
            var userId = await GetUserIdFromClaimsAsync();
            if (userId == null)
            {
                return Unauthorized(new { error = "Invalid user" });
            }
            
            var storyGuid = ProgressControllerHelpers.ConvertStoryIdToGuid(storyId);

            var progress = await _chapterProgressService.GetUserStoryProgressAsync(userId.Value, storyGuid);

            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting story progress");
            return StatusCode(500, new { error = "Failed to get progress" });
        }
    }

    private async Task<Guid?> GetUserIdFromClaimsAsync()
    {
        // Firebase IDトークンからUIDを取得
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
        {
            return null;
        }

        // Firebase UIDからユーザーを取得
        var firebaseAuthService = HttpContext.RequestServices.GetRequiredService<HackStory.Application.Services.IFirebaseAuthService>();
        var user = await firebaseAuthService.GetUserByFirebaseUidAsync(firebaseUid);
        return user?.Id;
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

