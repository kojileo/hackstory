using HackStory.Domain.Entities;

namespace HackStory.Application.Services;

public interface IStoryService
{
    Task<IEnumerable<Story>> GetAllStoriesAsync();
    Task<Story?> GetStoryByIdAsync(Guid id);
}

