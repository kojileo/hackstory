using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace HackStory.Api.Extensions;

public static class FirebaseExtensions
{
    public static IServiceCollection AddFirebase(this IServiceCollection services, IConfiguration configuration)
    {
        // Firebase Admin SDKの初期化
        if (FirebaseApp.DefaultInstance == null)
        {
            var firebaseConfig = configuration.GetSection("Firebase");
            var projectId = firebaseConfig["ProjectId"];
            var credentialsPath = firebaseConfig["CredentialsPath"];

            if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
            {
                // サービスアカウントキーファイルから初期化
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialsPath),
                    ProjectId = projectId
                });
            }
            else if (!string.IsNullOrEmpty(projectId))
            {
                // 環境変数から初期化（Google Cloud Run環境など）
                FirebaseApp.Create(new AppOptions()
                {
                    ProjectId = projectId
                });
            }
            else
            {
                // デフォルトの認証情報を使用（Google Cloud環境で自動検出）
                FirebaseApp.Create();
            }
        }

        return services;
    }
}

