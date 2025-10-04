using CarParkManager.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarParkManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CarParkDbContext>(opt => opt.UseInMemoryDatabase("CarParkDb"));
        services.AddScoped<IParkingSpaceRepository, ParkingSpaceRepository>();
        services.AddScoped<IParkingSessionRepository, ParkingSessionRepository>();
        return services;
    }
}
