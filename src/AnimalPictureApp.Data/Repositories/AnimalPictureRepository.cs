using AnimalPictureApp.Core.Data;
using AnimalPictureApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimalPictureApp.Data.Repositories;

public class AnimalPictureRepository : IAnimalPictureRepository
{
    private readonly AnimalPictureDbContext _dbContext;
    private readonly ILogger<AnimalPictureRepository> _logger;

    public AnimalPictureRepository(
        AnimalPictureDbContext dbContext,
        ILogger<AnimalPictureRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AnimalPicture> AddAsync(AnimalPicture picture)
    {
        try
        {
            var entry = await _dbContext.AnimalPictures.AddAsync(picture);
            return entry.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding picture for animal type: {AnimalType}", picture.AnimalType);
            throw;
        }
    }

    public async Task<AnimalPicture?> GetLatestByAnimalTypeAsync(string animalType)
    {
        try
        {
            return await _dbContext.AnimalPictures
                .Where(p => p.AnimalType.ToLower() == animalType.ToLower())
                .OrderByDescending(p => p.StoredAt)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving latest picture for animal type: {AnimalType}", animalType);
            throw;
        }
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }
}
