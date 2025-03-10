using AnimalPictureApp.Core.Models;
using AnimalPictureApp.Core.Data;
using Microsoft.Extensions.Logging;

namespace AnimalPictureApp.Core.Services;

public class AnimalPictureService : IAnimalPictureService
{
    private readonly IAnimalPictureRepository _repository;
    private readonly IImageDownloadService _imageDownloadService;
    private readonly ILogger<AnimalPictureService> _logger;

    public AnimalPictureService(
        IAnimalPictureRepository repository,
        IImageDownloadService imageDownloadService,
        ILogger<AnimalPictureService> logger)
    {
        _repository = repository;
        _imageDownloadService = imageDownloadService;
        _logger = logger;
    }

    public async Task<AnimalPicture> SavePicturesAsync(string animalType, int count)
    {
        _logger.LogInformation("Saving {Count} pictures for animal type: {AnimalType}", count, animalType);
        
        AnimalPicture lastSavedPicture = null;

        for (int i = 0; i < count; i++)
        {
            try
            {
                // Download image from the appropriate API
                var (imageData, contentType) = await _imageDownloadService.DownloadImageAsync(animalType);

                // Create and save the animal picture
                var picture = new AnimalPicture
                {
                    AnimalType = animalType,
                    ImageData = imageData,
                    ContentType = contentType,
                    StoredAt = DateTime.UtcNow
                };

                await _repository.AddAsync(picture);
                await _repository.SaveChangesAsync();

                lastSavedPicture = picture;
                _logger.LogInformation("Successfully saved picture {Current}/{Total} for {AnimalType}", i + 1, count, animalType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving picture {Current}/{Total} for {AnimalType}", i + 1, count, animalType);
                
                // If this is the first picture and it failed, we should propagate the error
                if (i == 0)
                {
                    throw;
                }
                
                // If we've saved at least one picture, we can break the loop and return the last successful one
                break;
            }
        }

        if (lastSavedPicture == null)
        {
            throw new InvalidOperationException($"Failed to save any pictures for animal type: {animalType}");
        }

        return lastSavedPicture;
    }

    public async Task<AnimalPicture?> GetLatestPictureAsync(string animalType)
    {
        _logger.LogInformation("Retrieving latest picture for animal type: {AnimalType}", animalType);

        try
        {
            return await _repository.GetLatestByAnimalTypeAsync(animalType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving latest picture for animal type: {AnimalType}", animalType);
            throw;
        }
    }
}
