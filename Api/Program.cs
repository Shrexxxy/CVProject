using BL;
using BL.Mapping;
using DAL;
using DAL.Equipment;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Equipment")
                       ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:Equipment.");

builder.Services.AddControllers();
builder.Services.AddEquipmentBusinessLogic();
builder.Services.AddEquipmentDataAccess(connectionString);
builder.Services.AddAutoMapper(typeof(EquipmentFilterProfile).Assembly, typeof(EquipmentMappingProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<EquipmentDbContext>().Database.MigrateAsync();
}

app.MapControllers();

app.Run();
