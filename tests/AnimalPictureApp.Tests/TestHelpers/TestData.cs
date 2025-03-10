using AnimalPictureApp.Core.Models;

namespace AnimalPictureApp.Tests.TestHelpers;

public static class TestData
{
    public static byte[] GetSampleImageData() => new byte[] { 1, 2, 3, 4, 5 };

    public static AnimalPicture CreateSampleAnimalPicture(string animalType = "cat")
    {
        return new AnimalPicture
        {
            Id = 1,
            AnimalType = animalType,
            ImageData = GetSampleImageData(),
            ContentType = "image/jpeg",
            StoredAt = DateTime.UtcNow
        };
    }

    public static List<AnimalPicture> CreateSampleAnimalPictures(string animalType = "cat", int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => new AnimalPicture
            {
                Id = i,
                AnimalType = animalType,
                ImageData = GetSampleImageData(),
                ContentType = "image/jpeg",
                StoredAt = DateTime.UtcNow.AddMinutes(-i)
            })
            .ToList();
    }
}
