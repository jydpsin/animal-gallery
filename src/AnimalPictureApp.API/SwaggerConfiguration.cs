using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AnimalPictureApp.API;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Animal Picture API",
                Version = "v1",
                Description = "A simple API for fetching and storing animal pictures",
                Contact = new OpenApiContact
                {
                    Name = "Animal Picture App",
                    Email = "support@example.com"
                }
            });

            // Include XML comments in Swagger documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
    }
}
