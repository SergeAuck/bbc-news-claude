using AusNews.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<INewsService, NewsService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

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
