using FinTracker.Data;
using FinTracker.Domain.Dtos.Universal;
using Mapster;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRepositories();
        builder.Services.AddServices();
        builder.Services.AddParser();
        builder.Services.AddContext();
        builder.Services.AddMemoryCache();
        MappingConfig.Configure();
        builder.Services.AddMapster();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        // Настройка сайтов, с которых могут обращаться к api
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Dev", policy =>
            {
                policy
                    .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Применение миграций
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        // Обработка ошибок
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

        app.UseCors("Dev");

        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.Run();
    }
}
