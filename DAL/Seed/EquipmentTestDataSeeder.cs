using BL.Abstractions;
using BL.Common;
using BL.Entities;
using Contracts.Equipment;
using Contracts.Maintenance;
using DAL.Equipment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Seed;

/// <summary>
/// Seed тестовых данных для локального запуска и проверки API через Swagger, только для Development
/// </summary>
public static class EquipmentTestDataSeeder
{
    /// <summary>
    /// Организация с основным набором оборудования
    /// </summary>
    public static readonly Guid OrganizationId = new("11111111-1111-1111-1111-111111111111");

    /// <summary>
    /// Вторая организация, чтобы было видно фильтр по организации
    /// </summary>
    public static readonly Guid OtherOrganizationId = new("44444444-4444-4444-4444-444444444444");

    /// <summary>
    /// Организация-подрядчик, обслуживающая насосы в марте 2026
    /// </summary>
    public static readonly Guid ContractorOrganizationId = new("22222222-2222-2222-2222-222222222222");

    /// <summary>
    /// Сотрудник, бессрочно обслуживающий компрессор с 2026 года
    /// </summary>
    public static readonly Guid ContractorEmployeeId = new("33333333-3333-3333-3333-333333333333");

    private static readonly Guid MainBuildingId = new("c0000000-0000-0000-0000-000000000001");
    private static readonly Guid WorkshopBuildingId = new("c0000000-0000-0000-0000-000000000002");
    private static readonly Guid OfficeBuildingId = new("c0000000-0000-0000-0000-000000000003");

    /// <summary>
    /// Заполнить пустую базу тестовыми данными; если оборудование уже есть, ничего не делает
    /// </summary>
    /// <param name="services">Провайдер сервисов текущего scope</param>
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<EquipmentDbContext>();
        if (await context.DocumentIndex.AnyAsync())
            return;

        var repository = services.GetRequiredService<IEquipmentRepository>();
        foreach (var equipment in CreateEquipment())
            await repository.Add(ToDocument(equipment));

        foreach (var maintenance in CreateMaintenance())
        {
            context.MaintenanceDocuments.Add(new Document<MaintenanceModel>
            {
                EntityId = maintenance.EntityId,
                Version = maintenance.Version,
                Timestamp = maintenance.Timestamp,
                Model = maintenance
            });

            context.MaintenanceDocumentIndex.Add(new MaintenanceEntity
            {
                EntityId = maintenance.EntityId,
                OrganizationId = maintenance.OrganizationId,
                EquipmentIds = maintenance.EquipmentIds.ToArray(),
                ContractorOrganizationId = maintenance.ContractorOrganizationId,
                ContractorEmployeeId = maintenance.ContractorEmployeeId,
                BeginDate = maintenance.BeginDate,
                EndDate = maintenance.EndDate
            });
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<EquipmentModel> CreateEquipment()
    {
        yield return new EquipmentModel
        {
            EntityId = EquipmentId(1), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Production,
            BuildingId = MainBuildingId, BuildingName = "Корпус 1",
            Name = "Насос погружной", ManufacturerName = "Завод насосного оборудования", Model = "НП-100",
            SerialNumber = "SN-1001", InventoryNumber = "INV-001",
            ReleaseDate = new DateTime(2019, 3, 1), ReleaseDateFormat = "m", InputExploitationDate = new DateTime(2019, 4, 15),
            Count = 1
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(2), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Production,
            BuildingId = MainBuildingId, BuildingName = "Корпус 1",
            Name = "Насос циркуляционный", ManufacturerName = "Завод насосного оборудования", Model = "ЦН-40",
            SerialNumber = "SN-1002", InventoryNumber = "INV-002",
            ReleaseDate = new DateTime(2020, 6, 10), InputExploitationDate = new DateTime(2020, 7, 1),
            Count = 1
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(3), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Production,
            BuildingId = WorkshopBuildingId, BuildingName = "Цех 2",
            Name = "Компрессор винтовой", ManufacturerName = "Компрессорный завод", Model = "КВ-7",
            SerialNumber = "SN-2001", InventoryNumber = "INV-003",
            ReleaseDate = new DateTime(2018, 1, 1), ReleaseDateFormat = "y", InputExploitationDate = new DateTime(2018, 9, 3),
            Count = 1
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(4), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Office,
            BuildingId = OfficeBuildingId, BuildingName = "Административный корпус",
            Name = "Ноутбук", ManufacturerName = "ТехноКомп", Model = "TK-15",
            SerialNumber = "SN-3001", InventoryNumber = "INV-004",
            ReleaseDate = new DateTime(2023, 2, 20), InputExploitationDate = new DateTime(2023, 3, 1),
            Count = 1
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(5), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Office,
            BuildingId = OfficeBuildingId, BuildingName = "Административный корпус",
            Name = "МФУ лазерное", ManufacturerName = "ПечатьТех", Model = "MFP-400",
            SerialNumber = "SN-3002", InventoryNumber = "INV-005",
            ReleaseDate = new DateTime(2022, 11, 5), InputExploitationDate = new DateTime(2022, 12, 1),
            Count = 1
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(6), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Vehicle,
            BuildingId = WorkshopBuildingId, BuildingName = "Цех 2",
            Name = "Погрузчик вилочный", ManufacturerName = "Завод складской техники", Model = "ПВ-15",
            SerialNumber = "SN-4001", InventoryNumber = "INV-006",
            ReleaseDate = new DateTime(2021, 5, 14), InputExploitationDate = new DateTime(2021, 6, 1),
            Count = 1
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(7), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Other,
            BuildingId = WorkshopBuildingId, BuildingName = "Цех 2",
            Name = "Генератор дизельный", ManufacturerName = "Энергомаш", Model = "ДГ-30",
            SerialNumber = "SN-5001", InventoryNumber = "INV-007",
            ReleaseDate = new DateTime(2012, 8, 1), InputExploitationDate = new DateTime(2012, 10, 1),
            OutputExploitationDate = new DateTime(2024, 5, 1), ReasonOutputExploitation = "Выработан ресурс",
            Count = 1
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(8), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Production,
            BuildingId = MainBuildingId, BuildingName = "Корпус 1",
            Name = "Станок токарный", ManufacturerName = "Станкозавод", Model = "СТ-16",
            SerialNumber = "SN-6001", InventoryNumber = "INV-008",
            ReleaseDate = new DateTime(2024, 1, 10), InputExploitationDate = new DateTime(2024, 2, 1),
            Count = 1, IsDraft = true
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(9), OrganizationId = OrganizationId, EquipmentType = EquipmentType.Office,
            BuildingId = OfficeBuildingId, BuildingName = "Административный корпус",
            Name = "Стул офисный", ManufacturerName = "Мебельная фабрика", Model = "Стандарт",
            SerialNumber = "б/н", InventoryNumber = "б/н",
            ReleaseDate = new DateTime(2021, 1, 1), ReleaseDateFormat = "y", InputExploitationDate = new DateTime(2021, 3, 1),
            Count = 12
        };

        yield return new EquipmentModel
        {
            EntityId = EquipmentId(10), OrganizationId = OtherOrganizationId, EquipmentType = EquipmentType.Production,
            BuildingName = "Склад",
            Name = "Насос погружной", ManufacturerName = "Завод насосного оборудования", Model = "НП-100",
            SerialNumber = "SN-9001", InventoryNumber = "INV-901",
            ReleaseDate = new DateTime(2020, 1, 1), InputExploitationDate = new DateTime(2020, 2, 1),
            Count = 1
        };
    }

    private static IEnumerable<MaintenanceModel> CreateMaintenance()
    {
        var timestamp = DateTime.UtcNow;

        yield return new MaintenanceModel
        {
            EntityId = new Guid("b0000000-0000-0000-0000-000000000001"), Version = 1, Timestamp = timestamp,
            OrganizationId = OrganizationId,
            EquipmentIds = [EquipmentId(1), EquipmentId(2)],
            ContractorOrganizationId = ContractorOrganizationId,
            BeginDate = new DateTime(2026, 3, 1), EndDate = new DateTime(2026, 3, 31)
        };

        yield return new MaintenanceModel
        {
            EntityId = new Guid("b0000000-0000-0000-0000-000000000002"), Version = 1, Timestamp = timestamp,
            OrganizationId = OrganizationId,
            EquipmentIds = [EquipmentId(3)],
            ContractorEmployeeId = ContractorEmployeeId,
            BeginDate = new DateTime(2026, 1, 1)
        };
    }

    private static Document<EquipmentModel> ToDocument(EquipmentModel model)
    {
        model.Version = 1;
        model.Timestamp = DateTime.UtcNow;

        return new Document<EquipmentModel>
        {
            EntityId = model.EntityId,
            Version = model.Version,
            Timestamp = model.Timestamp,
            Model = model
        };
    }

    private static Guid EquipmentId(int number)
    {
        return new Guid($"a0000000-0000-0000-0000-{number:D12}");
    }
}
