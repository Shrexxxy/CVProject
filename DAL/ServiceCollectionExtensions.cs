using BL.Abstractions;
using BL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DAL.Base;
using DAL.Equipment;

namespace DAL;

/// <summary>
/// Регистрация хранилища оборудования в DI
/// </summary>
public static class ServiceCollectionExtensions
{
    public static void AddEquipmentDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EquipmentDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.ConfigureDataSource(ds => ds.EnableDynamicJson())));
        services.AddScoped<IQueryBuilder<EquipmentEntity, EquipmentFilterSettings>, EquipmentQueryBuilder>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
    }
}
