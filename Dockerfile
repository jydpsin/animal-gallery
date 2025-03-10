FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/AnimalPictureApp.API/AnimalPictureApp.API.csproj", "AnimalPictureApp.API/"]
COPY ["src/AnimalPictureApp.Core/AnimalPictureApp.Core.csproj", "AnimalPictureApp.Core/"]
COPY ["src/AnimalPictureApp.Data/AnimalPictureApp.Data.csproj", "AnimalPictureApp.Data/"]

# Restore packages
RUN dotnet restore "AnimalPictureApp.API/AnimalPictureApp.API.csproj"

# Copy the source code
COPY src/. .

# Build and publish
RUN dotnet build "AnimalPictureApp.API/AnimalPictureApp.API.csproj" -c Release -o /app/build
RUN dotnet publish "AnimalPictureApp.API/AnimalPictureApp.API.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Install SQLite
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Create directory for SQLite database
RUN mkdir -p /app/Data

# Expose port 80
EXPOSE 80

ENTRYPOINT ["dotnet", "AnimalPictureApp.API.dll"]
