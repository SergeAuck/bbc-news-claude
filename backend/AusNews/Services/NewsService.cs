using System.Text.Json;
using AusNews.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AusNews.Services;

public interface INewsService
{
    Task<NewsApiResponse> GetTopHeadlinesAsync(string source = "bbc-news");
}

public class NewsService : INewsService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NewsService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public NewsService(
        HttpClient httpClient,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<NewsService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<NewsApiResponse> GetTopHeadlinesAsync(string source = "bbc-news")
    {
        var cacheKey = $"news_{source}";

        if (_cache.TryGetValue(cacheKey, out NewsApiResponse? cached) && cached is not null)
        {
            _logger.LogInformation("Returning cached news for source: {Source}", source);
            return cached;
        }

        var apiKey = _configuration["NewsApi:ApiKey"]
            ?? throw new InvalidOperationException("NewsApi:ApiKey is not configured");
        var baseUrl = _configuration["NewsApi:BaseUrl"] ?? "https://newsapi.org/v2";
        var cacheMinutes = _configuration.GetValue<int>("NewsApi:CacheMinutes", 3);

        var url = $"{baseUrl}/top-headlines?sources={Uri.EscapeDataString(source)}&apiKey={apiKey}";

        _logger.LogInformation("Fetching fresh news for source: {Source}", source);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var newsResponse = JsonSerializer.Deserialize<NewsApiResponse>(json, JsonOptions)
            ?? new NewsApiResponse();

        _cache.Set(cacheKey, newsResponse, TimeSpan.FromMinutes(cacheMinutes));

        return newsResponse;
    }
}
