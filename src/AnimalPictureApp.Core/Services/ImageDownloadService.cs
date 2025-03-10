using AnimalPictureApp.Core.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnimalPictureApp.Core.Services;

public class ImageDownloadService : IImageDownloadService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ImageDownloadService> _logger;
    private readonly Dictionary<string, (string Url, string Fallback)> _apiUrls;

    public ImageDownloadService(
        IHttpClientFactory httpClientFactory,
        ILogger<ImageDownloadService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiUrls = new Dictionary<string, (string Url, string Fallback)>(StringComparer.OrdinalIgnoreCase)
        {
            { "cat", ("https://api.thecatapi.com/v1/images/search", "https://cataas.com/cat") },
            { "dog", ("https://dog.ceo/api/breeds/image/random", "https://placedog.net/500") },
            { "bear", ("https://api.unsplash.com/photos/random?query=bear&client_id=demo", "https://placebear.com/500/500") }
        };
    }

    public async Task<(byte[] imageData, string contentType)> DownloadImageAsync(string animalType, int width = 400, int height = 400)
    {
        if (!_apiUrls.TryGetValue(animalType, out var urls))
        {
            throw new ArgumentException($"Unsupported animal type: {animalType}", nameof(animalType));
        }

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);
        
        string imageUrl;
        try
        {
            imageUrl = animalType switch
            {
                "dog" => await GetDogImageUrl(client),
                "cat" => await GetCatImageUrl(client),
                "bear" => await GetBearImageUrl(client),
                _ => throw new ArgumentException($"Unsupported animal type: {animalType}", nameof(animalType))
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get image URL from primary API for {AnimalType}, using fallback", animalType);
            imageUrl = _apiUrls[animalType].Fallback;
        }
        
        try
        {
            _logger.LogInformation("Downloading image from {Url}", imageUrl);
            
            using var response = await client.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var imageData = await response.Content.ReadAsByteArrayAsync();

            if (imageData.Length == 0)
            {
                throw new InvalidOperationException("Received empty image data");
            }

            return (imageData, contentType);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error downloading image from {Url}", imageUrl);
            throw new InvalidOperationException($"Failed to download image from {imageUrl}", ex);
        }
    }

    private async Task<string> GetDogImageUrl(HttpClient client)
    {
        try
        {
            var response = await client.GetAsync(_apiUrls["dog"].Url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var dogResponse = JsonSerializer.Deserialize<DogApiResponse>(content);
            
            return dogResponse?.Message ?? throw new InvalidOperationException("Invalid response from Dog API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dog image URL");
            throw new InvalidOperationException("Failed to get dog image URL", ex);
        }
    }

    private class DogApiResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    private class CatApiResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    private class UnsplashResponse
    {
        [JsonPropertyName("urls")]
        public UnsplashUrls Urls { get; set; } = new();
    }

    private class UnsplashUrls
    {
        [JsonPropertyName("regular")]
        public string Regular { get; set; } = string.Empty;
    }

    private async Task<string> GetCatImageUrl(HttpClient client)
    {
        try
        {
            var response = await client.GetAsync(_apiUrls["cat"].Url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var catResponses = JsonSerializer.Deserialize<CatApiResponse[]>(content);
            
            return catResponses?.FirstOrDefault()?.Url 
                ?? throw new InvalidOperationException("Invalid response from Cat API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cat image URL");
            throw new InvalidOperationException("Failed to get cat image URL", ex);
        }
    }

    private async Task<string> GetBearImageUrl(HttpClient client)
    {
        try
        {
            var response = await client.GetAsync(_apiUrls["bear"].Url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var bearResponse = JsonSerializer.Deserialize<UnsplashResponse>(content);
            
            return bearResponse?.Urls.Regular 
                ?? throw new InvalidOperationException("Invalid response from Bear API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bear image URL");
            throw new InvalidOperationException("Failed to get bear image URL", ex);
        }
    }
}
