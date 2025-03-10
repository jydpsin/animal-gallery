namespace AnimalPictureApp.Core.Models;

public class AnimalPictureResponse
{
    /// <summary>
    /// Unique identifier for the animal picture
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Type of animal (cat, dog, or bear)
    /// </summary>
    public string AnimalType { get; set; } = string.Empty;

    /// <summary>
    /// Base64 encoded image data
    /// </summary>
    public string ImageData { get; set; } = string.Empty;

    /// <summary>
    /// Content type of the image (e.g., image/jpeg)
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the picture was stored
    /// </summary>
    public DateTime StoredAt { get; set; }
}
