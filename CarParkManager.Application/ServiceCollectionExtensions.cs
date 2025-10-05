using CarParkManager.Application.Services;
using FluentValidation;
using CarParkManager.Application.Validation;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using CarParkManager.Domain;

namespace CarParkManager.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddSingleton<IChargeCalculator,ChargeCalculator>();
        services.AddScoped<IParkingService, ParkingService>();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<AllocateRequestValidator>();
        return services;
    }
}
