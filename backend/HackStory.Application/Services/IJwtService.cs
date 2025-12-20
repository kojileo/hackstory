using HackStory.Domain.Entities;

namespace HackStory.Application.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

