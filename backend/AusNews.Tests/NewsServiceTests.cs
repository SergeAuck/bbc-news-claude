using System.Net;
using System.Text.Json;
using AusNews.Models;
using AusNews.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AusNews.Tests;

public class NewsServiceTests
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<NewsService>> _loggerMock;

    public NewsServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<NewsService>>();

        var configData = new Dictionary<string, string?>
        {
            ["NewsApi:ApiKey"] = "test-api-key",
            ["NewsApi:BaseUrl"] = "https://newsapi.org/v2",
            ["NewsApi:CacheMinutes"] = "3"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_ReturnsArticles()
    {
        var expectedResponse = new NewsApiResponse
        {
            Status = "ok",
            TotalResults = 1,
            Articles =
            [
                new Article
                {
                    Title = "Test Article",
                    Description = "Test Description",
                    Url = "https://example.com",
                    PublishedAt = DateTime.UtcNow
                }
            ]
        };

        var json = JsonSerializer.Serialize(expectedResponse);
        var httpClient = CreateMockHttpClient(json, HttpStatusCode.OK);

        var service = new NewsService(httpClient, _cache, _configuration, _loggerMock.Object);
        var result = await service.GetTopHeadlinesAsync();

        Assert.Equal("ok", result.Status);
        Assert.Single(result.Articles);
        Assert.Equal("Test Article", result.Articles[0].Title);
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_ReturnsCachedData_OnSecondCall()
    {
        var expectedResponse = new NewsApiResponse
        {
            Status = "ok",
            TotalResults = 1,
            Articles = [new Article { Title = "Cached Article" }]
        };

        var json = JsonSerializer.Serialize(expectedResponse);
        var httpClient = CreateMockHttpClient(json, HttpStatusCode.OK);

        var service = new NewsService(httpClient, _cache, _configuration, _loggerMock.Object);

        var result1 = await service.GetTopHeadlinesAsync();
        var result2 = await service.GetTopHeadlinesAsync();

        Assert.Equal("Cached Article", result1.Articles[0].Title);
        Assert.Equal("Cached Article", result2.Articles[0].Title);
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_ThrowsWhenApiKeyMissing()
    {
        var emptyConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var httpClient = CreateMockHttpClient("{}", HttpStatusCode.OK);
        var service = new NewsService(httpClient, _cache, emptyConfig, _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetTopHeadlinesAsync());
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_ThrowsOnHttpError()
    {
        var httpClient = CreateMockHttpClient("", HttpStatusCode.InternalServerError);
        var service = new NewsService(httpClient, _cache, _configuration, _loggerMock.Object);

        // Clear cache to force HTTP call
        _cache.Remove("news_bbc-news");

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetTopHeadlinesAsync());
    }

    private static HttpClient CreateMockHttpClient(string content, HttpStatusCode statusCode)
    {
        var handler = new MockHttpMessageHandler(content, statusCode);
        return new HttpClient(handler) { BaseAddress = new Uri("https://newsapi.org") };
    }
}

public class MockHttpMessageHandler(string content, HttpStatusCode statusCode) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        });
    }
}
