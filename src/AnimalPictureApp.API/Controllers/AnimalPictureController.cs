using Microsoft.AspNetCore.Mvc;
using AnimalPictureApp.Core.Models;
using AnimalPictureApp.Core.Services;

namespace AnimalPictureApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalPictureController : ControllerBase
{
    private readonly IAnimalPictureService _animalPictureService;
    private readonly ILogger<AnimalPictureController> _logger;

    public AnimalPictureController(
        IAnimalPictureService animalPictureService,
        ILogger<AnimalPictureController> logger)
    {
        _animalPictureService = animalPictureService;
        _logger = logger;
    }

    /// <summary>
    /// Saves pictures for the specified animal type
    /// </summary>
    /// <param name="animalType">Type of animal (cat, dog, or bear)</param>
    /// <param name="count">Number of pictures to fetch and save (default: 1)</param>
    /// <returns>The last saved picture with its metadata and Base64 encoded image data</returns>
    /// <response code="200">Returns the last saved picture</response>
    /// <response code="400">If the animal type is invalid or count is out of range (1-10)</response>
    /// <response code="500">If there was an error saving the pictures</response>
    [HttpPost("{animalType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnimalPictureResponse>> SavePictures(
        string animalType,
        [FromQuery] int count = 1)
    {
        try
        {
            if (!IsValidAnimalType(animalType))
            {
                return BadRequest($"Invalid animal type. Supported types are: {string.Join(", ", Enum.GetNames<AnimalType>())}");
            }

            if (count < 1 || count > 10)
            {
                return BadRequest("Count must be between 1 and 10");
            }

            var picture = await _animalPictureService.SavePicturesAsync(animalType.ToLower(), count);
            return Ok(MapToResponse(picture));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving pictures for animal type: {AnimalType}", animalType);
            return StatusCode(500, "An error occurred while saving the pictures");
        }
    }

    /// <summary>
    /// Gets the latest picture for the specified animal type
    /// </summary>
    /// <param name="animalType">Type of animal (cat, dog, or bear)</param>
    /// <returns>The latest picture with its metadata and Base64 encoded image data</returns>
    /// <response code="200">Returns the latest picture for the specified animal type</response>
    /// <response code="400">If the animal type is invalid</response>
    /// <response code="404">If no pictures are found for the specified animal type</response>
    /// <response code="500">If there was an error retrieving the picture</response>
    [HttpGet("{animalType}/latest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnimalPictureResponse>> GetLatestPicture(string animalType)
    {
        try
        {
            if (!IsValidAnimalType(animalType))
            {
                return BadRequest($"Invalid animal type. Supported types are: {string.Join(", ", Enum.GetNames<AnimalType>())}");
            }

            var picture = await _animalPictureService.GetLatestPictureAsync(animalType.ToLower());
            
            if (picture == null)
            {
                return NotFound($"No pictures found for animal type: {animalType}");
            }

            return Ok(MapToResponse(picture));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving latest picture for animal type: {AnimalType}", animalType);
            return StatusCode(500, "An error occurred while retrieving the picture");
        }
    }

    private static bool IsValidAnimalType(string animalType)
    {
        return Enum.TryParse<AnimalType>(animalType, true, out _);
    }

    private static AnimalPictureResponse MapToResponse(AnimalPicture picture)
    {
        return new AnimalPictureResponse
        {
            Id = picture.Id,
            AnimalType = picture.AnimalType,
            ImageData = Convert.ToBase64String(picture.ImageData),
            ContentType = picture.ContentType,
            StoredAt = picture.StoredAt
        };
    }
}
