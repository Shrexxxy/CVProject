using BL.Abstractions;
using BL.Services;
using BL.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BL;

/// <summary>
/// Регистрация сервисов и валидаторов
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEquipmentBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddValidatorsFromAssemblyContaining<EquipmentFilterParametersValidator>();

        return services;
    }
}
