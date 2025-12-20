using HackStory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HackStory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IFirebaseAuthService firebaseAuthService,
        ILogger<AuthController> logger)
    {
        _firebaseAuthService = firebaseAuthService;
        _logger = logger;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyToken([FromBody] VerifyTokenRequest request)
    {
        try
        {
            // Firebase IDトークンを検証
            var firebaseUid = await _firebaseAuthService.VerifyIdTokenAsync(request.IdToken);

            // Firebase認証情報からユーザー情報を取得
            var firebaseUser = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(firebaseUid);
            
            // ユーザーを取得または作成
            var user = await _firebaseAuthService.GetOrCreateUserAsync(
                firebaseUid,
                firebaseUser.Email ?? string.Empty,
                firebaseUser.DisplayName);

            return Ok(new
            {
                success = true,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    username = user.Username,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Firebase token");
            return StatusCode(500, new { error = "Token verification failed" });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            // Firebase IDトークンからUIDを取得
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized(new { error = "Invalid token" });
            }

            var user = await _firebaseAuthService.GetUserByFirebaseUidAsync(firebaseUid);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                username = user.Username,
                createdAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { error = "Failed to get user" });
        }
    }
}

public class VerifyTokenRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? Username { get; set; }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

