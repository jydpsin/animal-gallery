using AnimalPictureApp.Data;
using Microsoft.EntityFrameworkCore;

namespace AnimalPictureApp.Tests.TestHelpers;

public static class DatabaseHelper
{
    public static AnimalPictureDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AnimalPictureDbContext>()
            .UseInMemoryDatabase(databaseName: $"AnimalPictureDb_{Guid.NewGuid()}")
            .Options;

        return new AnimalPictureDbContext(options);
    }

    public static async Task SeedTestData(AnimalPictureDbContext context, string animalType = "cat", int count = 3)
    {
        var pictures = TestData.CreateSampleAnimalPictures(animalType, count);
        await context.AnimalPictures.AddRangeAsync(pictures);
        await context.SaveChangesAsync();
    }
}
