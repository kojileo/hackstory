using HackStory.Domain.Entities;

namespace HackStory.Application.Services;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(string email, string password, string? username = null);
    Task<bool> ValidatePasswordAsync(User user, string password);
    Task<User?> GetUserByIdAsync(Guid userId);
}

