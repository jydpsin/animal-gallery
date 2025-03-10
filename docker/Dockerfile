FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/AnimalPictureApp.API/AnimalPictureApp.API.csproj", "src/AnimalPictureApp.API/"]
COPY ["src/AnimalPictureApp.Core/AnimalPictureApp.Core.csproj", "src/AnimalPictureApp.Core/"]
COPY ["src/AnimalPictureApp.Data/AnimalPictureApp.Data.csproj", "src/AnimalPictureApp.Data/"]
RUN dotnet restore "src/AnimalPictureApp.API/AnimalPictureApp.API.csproj"
COPY . .
WORKDIR "/src/src/AnimalPictureApp.API"
RUN dotnet build "AnimalPictureApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnimalPictureApp.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnimalPictureApp.API.dll"]
