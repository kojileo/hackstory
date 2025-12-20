using FirebaseAdmin;
using FirebaseAdmin.Auth;
using HackStory.Application.Interfaces;
using HackStory.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HackStory.Application.Services;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<FirebaseAuthService> _logger;

    public FirebaseAuthService(
        IRepository<User> userRepository,
        ILogger<FirebaseAuthService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<string> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            return decodedToken.Uid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Firebase ID token");
            throw new UnauthorizedAccessException("Invalid Firebase ID token", ex);
        }
    }

    public async Task<User?> GetOrCreateUserAsync(string firebaseUid, string email, string? displayName = null)
    {
        // 既存ユーザーを検索
        var existingUser = await GetUserByFirebaseUidAsync(firebaseUid);
        if (existingUser != null)
        {
            return existingUser;
        }

        // 新規ユーザーを作成
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = displayName,
            FirebaseUid = firebaseUid,
            PasswordHash = string.Empty, // Firebase認証ではパスワードハッシュは不要
            CreatedAt = DateTime.UtcNow
        };
        
        return await _userRepository.AddAsync(user);
    }

    public async Task<User?> GetUserByFirebaseUidAsync(string firebaseUid)
    {
        var users = await _userRepository.FindAsync(u => u.FirebaseUid == firebaseUid);
        return users.FirstOrDefault();
    }
}

