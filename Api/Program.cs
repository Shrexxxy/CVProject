using BL;
using BL.Mapping;
using DAL;
using DAL.Equipment;
using DAL.Seed;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

const string ApiName = "Equipment";
const int ApiVersion = 1;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Equipment")
                       ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:Equipment.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.ToString());
    c.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
    c.SwaggerDoc($"v{ApiVersion}", new OpenApiInfo
    {
        Version = $"v{ApiVersion}",
        Title = $"{ApiName} API",
    });
    
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return [api.GroupName];
        }
        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return [controllerActionDescriptor.ControllerName];
        }
        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });

    c.DocInclusionPredicate((_, _) => true);

    foreach (var xmlFile in new[] { "Api.xml", "Contracts.xml" })
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile), includeControllerXmlComments: true);
});
builder.Services.AddEquipmentBusinessLogic();
builder.Services.AddEquipmentDataAccess(connectionString);
builder.Services.AddAutoMapper(typeof(EquipmentFilterProfile).Assembly, typeof(EquipmentMappingProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<EquipmentDbContext>().Database.MigrateAsync();
    await EquipmentTestDataSeeder.SeedAsync(scope.ServiceProvider);
}

app.MapControllers();

app.Run();
