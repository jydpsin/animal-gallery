using AnimalPictureApp.Core.Data;
using AnimalPictureApp.Core.Services;
using AnimalPictureApp.Data;
using AnimalPictureApp.Data.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace AnimalPictureApp.Tests.TestHelpers;

public abstract class TestBase : IDisposable
{
    protected readonly AnimalPictureDbContext DbContext;
    protected readonly Mock<ILogger<AnimalPictureService>> ServiceLoggerMock;
    protected readonly Mock<ILogger<ImageDownloadService>> DownloadLoggerMock;
    protected readonly Mock<ILogger<AnimalPictureRepository>> RepositoryLoggerMock;
    protected readonly Mock<IHttpClientFactory> HttpClientFactoryMock;
    protected readonly IAnimalPictureRepository Repository;
    protected readonly ImageDownloadService ImageDownloadService;
    protected readonly AnimalPictureService AnimalPictureService;

    protected TestBase()
    {
        DbContext = DatabaseHelper.CreateInMemoryContext();
        ServiceLoggerMock = new Mock<ILogger<AnimalPictureService>>();
        DownloadLoggerMock = new Mock<ILogger<ImageDownloadService>>();
        RepositoryLoggerMock = new Mock<ILogger<AnimalPictureRepository>>();
        HttpClientFactoryMock = new Mock<IHttpClientFactory>();

        ImageDownloadService = new ImageDownloadService(
            HttpClientFactoryMock.Object,
            DownloadLoggerMock.Object);

        Repository = new AnimalPictureRepository(
            DbContext,
            RepositoryLoggerMock.Object);

        AnimalPictureService = new AnimalPictureService(
            Repository,
            ImageDownloadService,
            ServiceLoggerMock.Object);
    }

    protected static HttpClient CreateMockHttpClient(byte[] content, string contentType = "image/jpeg")
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new ByteArrayContent(content)
            });

        var client = new HttpClient(handler.Object);
        client.DefaultRequestHeaders.Add("ContentType", contentType);
        return client;
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}
