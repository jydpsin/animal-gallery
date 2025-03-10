namespace AnimalPictureApp.Core.Models
{
    public class AnimalPicture
    {
        public int Id { get; set; }
        public string AnimalType { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public DateTime StoredAt { get; set; }
        public string ContentType { get; set; } = "image/jpeg";
    }

    public enum AnimalType
    {
        Cat,
        Dog,
        Bear
    }
}
