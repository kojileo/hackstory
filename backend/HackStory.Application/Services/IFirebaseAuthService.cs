using HackStory.Domain.Entities;

namespace HackStory.Application.Services;

public interface IFirebaseAuthService
{
    Task<string> VerifyIdTokenAsync(string idToken);
    Task<User?> GetOrCreateUserAsync(string firebaseUid, string email, string? displayName = null);
    Task<User?> GetUserByFirebaseUidAsync(string firebaseUid);
}

