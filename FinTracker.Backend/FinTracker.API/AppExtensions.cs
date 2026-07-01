using FinTracker.Application;
using FinTracker.Data;
using FinTracker.Domain.Dtos.Universal;
using Microsoft.AspNetCore.Diagnostics;

namespace FinTracker.API;

public static class AppExtensions
{
    public static WebApplication AddExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appBuilder => appBuilder.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            var (statusCode, message) = exception switch
            {
                KeyNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
                ArgumentException ex => (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal server error")
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(message));
        }));

        return app;
    }

    public static IServiceCollection AddDevCors(this IServiceCollection serviceCollection, string[] devOrigins)
    {
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("Dev", policy =>
            {
                policy
                    .WithOrigins(devOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        return serviceCollection;
    }

    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!;

        builder.Services.AddDevCors(allowedOrigins);  // Настройка сайтов, с которых могут обращаться к api

        builder.Services.AddRepositories();
        builder.Services.AddServices();

        builder.Services.AddParser();

        builder.Services.AddContext();

        builder.Services.AddMemoryCache();

        builder.Services.AddMapper();

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplication ConfigureApp(this WebApplication app)
    {
        app.AddExceptionHandler();  // Обработка ошибок
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("Dev");
        app.MapControllers();

        return app;
    }
}