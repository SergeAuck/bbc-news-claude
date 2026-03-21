using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AusNews.Health;

public class NewsApiHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public NewsApiHealthCheck(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["NewsApi:ApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
        {
            return HealthCheckResult.Unhealthy("NewsAPI key is not configured.");
        }

        var baseUrl = _configuration["NewsApi:BaseUrl"] ?? "https://newsapi.org/v2";

        try
        {
            var response = await _httpClient.GetAsync(
                $"{baseUrl}/top-headlines?sources=bbc-news&pageSize=1&apiKey={apiKey}",
                cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("NewsAPI is reachable.")
                : HealthCheckResult.Degraded($"NewsAPI returned {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("NewsAPI is unreachable.", ex);
        }
    }
}
