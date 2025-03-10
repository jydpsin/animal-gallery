using AnimalPictureApp.Core.Models;

namespace AnimalPictureApp.Core.Services
{
    public interface IAnimalPictureService
    {
        Task<AnimalPicture> SavePicturesAsync(string animalType, int count);
        Task<AnimalPicture?> GetLatestPictureAsync(string animalType);
    }
}
