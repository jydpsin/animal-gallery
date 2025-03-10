using AnimalPictureApp.Core.Models;
using AnimalPictureApp.Core.Services;
using AnimalPictureApp.Core.Data;
using AnimalPictureApp.Data;
using AnimalPictureApp.Data.Repositories;
using AnimalPictureApp.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnimalPictureApp.Tests.Services;

public class AnimalPictureServiceTests : IDisposable
{
    private readonly Mock<IImageDownloadService> _imageDownloadServiceMock;
    private readonly Mock<ILogger<AnimalPictureService>> _loggerMock;
    private readonly Mock<IAnimalPictureRepository> _repositoryMock;
    private readonly AnimalPictureService _service;

    public AnimalPictureServiceTests()
    {
        _imageDownloadServiceMock = new Mock<IImageDownloadService>();
        _loggerMock = new Mock<ILogger<AnimalPictureService>>();
        _repositoryMock = new Mock<IAnimalPictureRepository>();
        _service = new AnimalPictureService(_repositoryMock.Object, _imageDownloadServiceMock.Object, _loggerMock.Object);
    }

    [Theory]
    [InlineData("cat", 1)]
    [InlineData("dog", 3)]
    [InlineData("bear", 5)]
    public async Task SavePicturesAsync_ValidInput_SavesAllPictures(string animalType, int count)
    {
        // Arrange
        var imageData = TestData.GetSampleImageData();
        _imageDownloadServiceMock
            .Setup(x => x.DownloadImageAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((imageData, "image/jpeg"));

        // Act
        var result = await _service.SavePicturesAsync(animalType, count);

        // Assert
        result.Should().NotBeNull();
        result.AnimalType.Should().Be(animalType);
        result.ImageData.Should().BeEquivalentTo(imageData);

        // Verify all pictures were saved
        _repositoryMock.Verify(x => x.AddAsync(It.Is<AnimalPicture>(p => p.AnimalType == animalType)), Times.Exactly(count));
        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(count));
    }

    [Fact]
    public async Task SavePicturesAsync_DownloadFailsForSomePictures_SavesPartialSuccess()
    {
        // Arrange
        var animalType = "cat";
        var count = 3;
        var imageData = TestData.GetSampleImageData();

        _imageDownloadServiceMock
            .SetupSequence(x => x.DownloadImageAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((imageData, "image/jpeg"))
            .ThrowsAsync(new Exception("Download failed"))
            .ReturnsAsync((imageData, "image/jpeg"));

        // Act
        var result = await _service.SavePicturesAsync(animalType, count);

        // Assert
        result.Should().NotBeNull();
        
        _repositoryMock.Verify(x => x.AddAsync(It.Is<AnimalPicture>(p => p.AnimalType == animalType)), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once); // Only successful downloads should be saved
    }

    [Fact]
    public async Task SavePicturesAsync_FirstDownloadFails_ThrowsException()
    {
        // Arrange
        _imageDownloadServiceMock
            .Setup(x => x.DownloadImageAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Download failed"));

        // Act
        var act = () => _service.SavePicturesAsync("cat", 1);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Download failed");

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<AnimalPicture>()), Times.Never);
        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("cat")]
    [InlineData("dog")]
    [InlineData("bear")]
    public async Task GetLatestPictureAsync_PicturesExist_ReturnsLatestPicture(string animalType)
    {
        // Arrange
        var samplePicture = new AnimalPicture
        {
            Id = 1,
            AnimalType = animalType,
            ImageData = TestData.GetSampleImageData(),
            ContentType = "image/jpeg",
            StoredAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(x => x.GetLatestByAnimalTypeAsync(animalType))
            .ReturnsAsync(samplePicture);

        // Act
        var result = await _service.GetLatestPictureAsync(animalType);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1); // Sample picture ID
        result.AnimalType.Should().Be(animalType);
    }

    [Fact]
    public async Task GetLatestPictureAsync_NoPicturesExist_ReturnsNull()
    {
        // Act
        var result = await _service.GetLatestPictureAsync("cat");

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        // No cleanup needed with mocked repository
        GC.SuppressFinalize(this);
    }
}
