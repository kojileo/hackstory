using HackStory.Application.Interfaces;
using HackStory.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace HackStory.Application.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IRepository<User> userRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email);
        return users.FirstOrDefault();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User> CreateUserAsync(string email, string password, string? username = null)
    {
        // メールアドレスの重複チェック
        var existingUser = await GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = HashPassword(password),
            Username = username,
            CreatedAt = DateTime.UtcNow
        };

        return await _userRepository.AddAsync(user);
    }

    public async Task<bool> ValidatePasswordAsync(User user, string password)
    {
        var hashedPassword = HashPassword(password);
        return user.PasswordHash == hashedPassword;
    }

    private static string HashPassword(string password)
    {
        // 簡易的なパスワードハッシュ化（本番環境ではBCryptやArgon2を使用推奨）
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

