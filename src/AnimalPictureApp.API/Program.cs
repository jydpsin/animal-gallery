using Microsoft.EntityFrameworkCore;
using AnimalPictureApp.Data;
using AnimalPictureApp.Data.Repositories;
using AnimalPictureApp.Core.Data;
using AnimalPictureApp.Core.Services;
using AnimalPictureApp.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with XML documentation
builder.Services.ConfigureSwagger();

// Configure XML documentation
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddXmlSerializerFormatters()
.AddXmlDataContractSerializerFormatters();

// Configure SQLite
builder.Services.AddDbContext<AnimalPictureDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IAnimalPictureRepository, AnimalPictureRepository>();
builder.Services.AddScoped<IAnimalPictureService, AnimalPictureService>();
builder.Services.AddScoped<IImageDownloadService, ImageDownloadService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Enable CORS
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure static files and default page
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AnimalPictureDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
