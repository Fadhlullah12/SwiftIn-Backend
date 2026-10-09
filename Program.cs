using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SwiftIn.Context;
using SwiftIn.Models.Entities;
using SwiftIn.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Connection string for the database
builder.Services.AddDbContext<SwiftInContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ConnectionString"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConnectionString"))));

//Registering Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SwiftInContext>()
    .AddDefaultTokenProviders();

// Registering repositories
builder.Services.AddRepositories();    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
