using AnimalPictureApp.API.Controllers;
using AnimalPictureApp.Core.Models;
using AnimalPictureApp.Core.Services;
using AnimalPictureApp.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace AnimalPictureApp.Tests.Controllers;

public class AnimalPictureControllerTests : TestBase
{
    private readonly Mock<ILogger<AnimalPictureController>> _loggerMock;
    private readonly Mock<IAnimalPictureService> _serviceMock;
    private readonly AnimalPictureController _controller;

    public AnimalPictureControllerTests()
    {
        _loggerMock = new Mock<ILogger<AnimalPictureController>>();
        _serviceMock = new Mock<IAnimalPictureService>();
        _controller = new AnimalPictureController(_serviceMock.Object, _loggerMock.Object);
    }

    [Theory]
    [InlineData("cat", 1)]
    [InlineData("dog", 5)]
    [InlineData("bear", 10)]
    public async Task SavePictures_ValidRequest_ReturnsOkWithPicture(string animalType, int count)
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

        _serviceMock.Setup(x => x.SavePicturesAsync(animalType, count))
            .ReturnsAsync(samplePicture);

        // Act
        var result = await _controller.SavePictures(animalType, count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AnimalPictureResponse>().Subject;

        response.AnimalType.Should().Be(animalType);
        response.ImageData.Should().NotBeNullOrEmpty();
        response.ContentType.Should().Be("image/jpeg");

        // Verify service was called
        _serviceMock.Verify(x => x.SavePicturesAsync(animalType, count), Times.Once);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")] // Empty string test case
    public async Task SavePictures_InvalidAnimalType_ReturnsBadRequest(string animalType)
    {
        // Act
        var result = await _controller.SavePictures(animalType, 1);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public async Task SavePictures_InvalidCount_ReturnsBadRequest(int count)
    {
        // Act
        var result = await _controller.SavePictures("cat", count);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SavePictures_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _serviceMock.Setup(x => x.SavePicturesAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.SavePictures("cat", 1);

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    [Theory]
    [InlineData("cat")]
    [InlineData("dog")]
    [InlineData("bear")]
    public async Task GetLatestPicture_PictureExists_ReturnsOkWithPicture(string animalType)
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

        _serviceMock.Setup(x => x.GetLatestPictureAsync(animalType))
            .ReturnsAsync(samplePicture);

        // Act
        var result = await _controller.GetLatestPicture(animalType);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AnimalPictureResponse>().Subject;

        response.AnimalType.Should().Be(animalType);
        response.ImageData.Should().NotBeNullOrEmpty();

        // Verify service was called
        _serviceMock.Verify(x => x.GetLatestPictureAsync(animalType), Times.Once);
    }

    [Fact]
    public async Task GetLatestPicture_PictureNotFound_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetLatestPicture("cat");

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        
        // Verify service was called
        _serviceMock.Verify(x => x.GetLatestPictureAsync("cat"), Times.Once);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")] // Empty string test case
    public async Task GetLatestPicture_InvalidAnimalType_ReturnsBadRequest(string animalType)
    {
        // Act
        var result = await _controller.GetLatestPicture(animalType);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetLatestPicture_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _serviceMock.Setup(x => x.GetLatestPictureAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetLatestPicture("cat");

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }
}
