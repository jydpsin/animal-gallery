using AnimalPictureApp.Core.Models;

namespace AnimalPictureApp.Core.Data;

public interface IAnimalPictureRepository
{
    Task<AnimalPicture> AddAsync(AnimalPicture picture);
    Task<AnimalPicture?> GetLatestByAnimalTypeAsync(string animalType);
    Task SaveChangesAsync();
}
