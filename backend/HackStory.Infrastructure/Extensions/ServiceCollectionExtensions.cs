using HackStory.Application.Interfaces;
using HackStory.Application.Services;
using HackStory.Domain.Entities;
using HackStory.Infrastructure.Data;
using HackStory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HackStory.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database (接続文字列がある場合のみ)
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // CockroachDB用の設定
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                }));
        }
        else
        {
            // データベース接続がない場合でも動作するように、InMemoryデータベースを使用
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("HackStoryInMemory"));
        }

        // Memory Cache (コスト削減のためRedis不使用)
        services.AddMemoryCache();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IChapterProgressRepository, ChapterProgressRepository>();

        // Services
        services.AddScoped<IStoryService, StoryService>();
        services.AddScoped<IChapterProgressService, ChapterProgressService>();
        services.AddScoped<HackStory.Application.Services.IUserService, HackStory.Application.Services.UserService>();
        services.AddScoped<HackStory.Application.Services.IFirebaseAuthService, HackStory.Application.Services.FirebaseAuthService>();

        return services;
    }
}

