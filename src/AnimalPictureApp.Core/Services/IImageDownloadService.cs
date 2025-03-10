namespace AnimalPictureApp.Core.Services;

public interface IImageDownloadService
{
    Task<(byte[] imageData, string contentType)> DownloadImageAsync(string animalType, int width = 400, int height = 400);
}
