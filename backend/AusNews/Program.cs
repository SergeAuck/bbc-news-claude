using AusNews.Services;
using AusNews.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<INewsService, NewsService>();
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddCheck<NewsApiHealthCheck>("newsapi");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.MapGet("/api/news", async (INewsService newsService) =>
{
    try
    {
        var news = await newsService.GetTopHeadlinesAsync();
        return Results.Ok(news);
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem($"Failed to fetch news: {ex.Message}", statusCode: 502);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("GetNews");

app.Run();

public partial class Program { }
