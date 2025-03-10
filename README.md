# Animal Picture App

A .NET Core Web API that downloads and stores random pictures of cats, dogs, and bears. This microservice was developed as part of the Camunda technical challenge.

## Features

- Download animal pictures from multiple sources (with automatic fallback)
- Support for cats, dogs, and bears
- Batch picture downloads (1-10 pictures at a time)
- SQLite database for picture storage
- Swagger/OpenAPI documentation
- Comprehensive error handling and logging
- Repository pattern for data access
- Extensive unit test coverage

## Prerequisites

- Docker Desktop (latest version)
- Git

## Quick Start

1. Clone and navigate to the repository:
   ```bash
   git clone https://github.com/jydpsin/animal-gallery.git
   cd animal-gallery
   ```

2. Run the application using Docker:
   ```bash
   docker-compose up --build
   ```

3. Access the application:
   - Web UI: http://localhost:8080
   - Swagger Documentation: http://localhost:8080/swagger

## Using the Web Interface

1. Open http://localhost:8080 in your browser
2. Use the simple UI to interact with the API:
   - Select an animal type (cat, dog, or bear)
   - Choose the number of pictures (1-10)
   - Click "Save New Pictures" to download and save pictures
   - Click "Get Latest Picture" to view the most recent picture
3. The UI will display:
   - The last downloaded picture
   - Status messages for successful/failed operations
   - Picture metadata (ID, type, when it was stored)

## API Endpoints

1. Save Animal Pictures
   ```http
   POST /api/animalpicture/{animalType}?count={count}
   ```
   - `animalType`: cat, dog, or bear
   - `count`: number of pictures to save (1-10, default: 1)
   - Returns: Latest saved picture with metadata

2. Get Latest Picture
   ```http
   GET /api/animalpicture/{animalType}/latest
   ```
   - `animalType`: cat, dog, or bear
   - Returns: Latest picture for the specified animal type

## Example Usage

1. Save 3 cat pictures:
   ```bash
   curl -X POST "http://localhost:8080/api/animalpicture/cat?count=3"
   ```

2. Get the latest dog picture:
   ```bash
   curl "http://localhost:8080/api/animalpicture/dog/latest"
   ```

## View Database Content

To view the SQLite database content:

```bash
# List all pictures
docker-compose exec api sqlite3 /app/Data/animalpictures.db \
".headers on" ".mode column" \
"SELECT Id, AnimalType, ContentType, StoredAt FROM AnimalPictures ORDER BY StoredAt DESC;"

# Count by animal type
docker-compose exec api sqlite3 /app/Data/animalpictures.db \
"SELECT AnimalType, COUNT(*) as Count FROM AnimalPictures GROUP BY AnimalType;"
```

## Running Tests

First install dotnet (if not installed)

```bash
brew install dotnet latest
```

Then run

```bash
dotnet test
```

The test suite includes:
- Controller tests for input validation and error handling
- Service layer tests for business logic
- Repository tests using EF Core InMemory provider
- Integration tests for external API calls

## Architecture

- **API Layer**: HTTP request handling, input validation, and response mapping
- **Core Layer**: Business logic, interfaces, and service implementations
- **Data Layer**: Entity Framework Core with SQLite, repository pattern
- **Test Layer**: Unit tests using xUnit, Moq, and FluentAssertions

## Technologies

- .NET 9.0
- Entity Framework Core with SQLite
- Docker & Docker Compose
- xUnit, Moq, FluentAssertions
- Swagger/OpenAPI

## Error Handling

The API includes comprehensive error handling:
- Invalid animal types return 400 Bad Request
- Not found pictures return 404 Not Found
- Server errors return 500 Internal Server Error with details
- Automatic fallback to alternative APIs when primary sources fail

## Notes

- SQLite database is created automatically in the `Data` directory
- Images are stored as binary data with metadata
- External API calls use `IHttpClientFactory` for optimal connection handling
- Fallback mechanisms ensure high availability


## Project Structure

```
AnimalPictureApp/
├── src/
│   ├── AnimalPictureApp.API/        # Main API project
│   ├── AnimalPictureApp.Core/       # Business logic and models
│   └── AnimalPictureApp.Data/       # Database context and repositories
├── tests/
│   └── AnimalPictureApp.Tests/      # Unit and integration tests
└── docker/
    └── Dockerfile                   # Docker configuration
```
