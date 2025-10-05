using CarParkManager.Application;
using CarParkManager.Domain;
using CarParkManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ParkingRateSettings>(
    builder.Configuration.GetSection(nameof(ParkingRateSettings)));

builder.Services.AddControllers();
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarParkDbContext>();
    if (builder.Configuration["DatabaseProvider"] == "InMemory")
    {
        db.Database.EnsureCreated();
    }
    else
    {
        await db.Database.MigrateAsync();
    }
    
    SeedData.Seed(db, builder.Configuration);
}

app.UseRouting();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", opts =>
        opts.WithTitle("Car Park Management API")
            .WithDarkMode());
}

app.Run();
