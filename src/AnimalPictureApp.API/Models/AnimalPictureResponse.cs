using System.ComponentModel.DataAnnotations;

namespace AnimalPictureApp.API.Models;

/// <summary>
/// Represents an animal picture response with metadata and image data
/// </summary>
public class AnimalPictureResponse
{
    /// <summary>
    /// Unique identifier for the picture
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Type of animal in the picture (cat, dog, or bear)
    /// </summary>
    /// <example>cat</example>
    [Required]
    public string AnimalType { get; set; } = string.Empty;

    /// <summary>
    /// Base64 encoded image data
    /// </summary>
    /// <example>iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==</example>
    [Required]
    public string ImageData { get; set; } = string.Empty;

    /// <summary>
    /// MIME type of the image
    /// </summary>
    /// <example>image/jpeg</example>
    [Required]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the picture was stored
    /// </summary>
    /// <example>2025-03-10T12:40:16.632176Z</example>
    public DateTime StoredAt { get; set; }
}
