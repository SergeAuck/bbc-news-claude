using System.Net;
using System.Text.Json;
using AusNews.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using AusNews.Services;
using Moq;

namespace AusNews.Tests;

public class NewsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public NewsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetNews_ReturnsOk_WithArticles()
    {
        var mockService = new Mock<INewsService>();
        mockService.Setup(s => s.GetTopHeadlinesAsync(It.IsAny<string>()))
            .ReturnsAsync(new NewsApiResponse
            {
                Status = "ok",
                TotalResults = 1,
                Articles = [new Article { Title = "Integration Test Article" }]
            });

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(INewsService));
                if (descriptor != null) services.Remove(descriptor);

                services.AddSingleton(mockService.Object);
            });
        }).CreateClient();

        var response = await client.GetAsync("/api/news");
        var json = await response.Content.ReadAsStringAsync();
        var news = JsonSerializer.Deserialize<NewsApiResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(news);
        Assert.Equal("Integration Test Article", news.Articles[0].Title);
    }
}
