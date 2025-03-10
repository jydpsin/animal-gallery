using AnimalPictureApp.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace AnimalPictureApp.Tests.Services;

public class ImageDownloadServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<ImageDownloadService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly ImageDownloadService _service;

    public ImageDownloadServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<ImageDownloadService>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var client = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(client);

        _service = new ImageDownloadService(_httpClientFactoryMock.Object, _loggerMock.Object);
    }

    [Theory]
    [InlineData("cat")]
    [InlineData("dog")]
    [InlineData("bear")]
    public async Task DownloadImageAsync_ValidAnimalType_ReturnsImageData(string animalType)
    {
        // Arrange
        var expectedData = new byte[] { 1, 2, 3 };
        var expectedContentType = "image/jpeg";

        SetupMockHttpResponse(expectedData, expectedContentType);

        // Act
        var (imageData, contentType) = await _service.DownloadImageAsync(animalType);

        // Assert
        imageData.Should().BeEquivalentTo(expectedData);
        contentType.Should().Be(expectedContentType);
    }

    [Fact]
    public async Task DownloadImageAsync_InvalidAnimalType_ThrowsArgumentException()
    {
        // Arrange
        var invalidAnimalType = "invalid";

        // Act
        var act = () => _service.DownloadImageAsync(invalidAnimalType);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"*{invalidAnimalType}*");
    }

    [Fact]
    public async Task DownloadImageAsync_ApiReturnsError_ThrowsInvalidOperationException()
    {
        // Arrange
        SetupMockHttpResponse(statusCode: HttpStatusCode.InternalServerError);

        // Act
        var act = () => _service.DownloadImageAsync("cat");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DownloadImageAsync_ApiReturnsEmptyData_ThrowsInvalidOperationException()
    {
        // Arrange
        SetupMockHttpResponse(Array.Empty<byte>(), "image/jpeg");

        // Act
        var act = () => _service.DownloadImageAsync("cat");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*empty image data*");
    }

    private void SetupMockHttpResponse(byte[]? content = null, string contentType = "image/jpeg", HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = new HttpResponseMessage(statusCode);
        
        if (content != null)
        {
            response.Content = new ByteArrayContent(content);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        }

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }
}
