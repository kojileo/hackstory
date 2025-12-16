namespace HackStory.Api.Middleware;

public class CostOptimizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CostOptimizationMiddleware> _logger;

    public CostOptimizationMiddleware(
        RequestDelegate next,
        ILogger<CostOptimizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // リクエストサイズ制限（1MB）
        if (context.Request.ContentLength > 1_048_576) // 1MB
        {
            context.Response.StatusCode = 413; // Payload Too Large
            await context.Response.WriteAsync("Request payload too large");
            return;
        }

        // レスポンスキャッシュヘッダーの設定
        context.Response.Headers.Append("Cache-Control", "public, max-age=300"); // 5分

        await _next(context);
    }
}

