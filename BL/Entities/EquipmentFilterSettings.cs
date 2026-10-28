using Contracts.Equipment;
using BL.Common;

namespace BL.Entities;

/// <summary>
/// Фильтр списка оборудования
/// </summary>
public class EquipmentFilterSettings : PagingSortSettings
{
    /// <summary>
    /// ID организации к которой относятся оборудования
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Идентификаторы зданий
    /// </summary>
    public List<Guid> BuildingIds { get; set; } = [];

    /// <summary>
    /// Типы оборудования
    /// </summary>
    public List<EquipmentType> EquipmentTypes { get; set; } = [];

    /// <summary>
    /// Включить в выборку оборудование, выведенное из эксплуатации
    /// </summary>
    public bool IncludeExploitationEnded { get; set; } = true;

    /// <summary>
    /// Включить в выборку черновики
    /// </summary>
    public bool IncludeDraft { get; set; } = true;

    /// <summary>
    /// Часть наименования оборудования
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Часть наименования производителя
    /// </summary>
    public string? ManufacturerName { get; set; }

    /// <summary>
    /// Часть наименования модели
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Серийный номер
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Инвентарный номер
    /// </summary>
    public string? InventoryNumber { get; set; }

    /// <summary>
    /// Дата ввода в эксплуатацию, от (включительно)
    /// </summary>
    public DateTime? InputExploitationDateFrom { get; set; }

    /// <summary>
    /// Дата ввода в эксплуатацию, до (включительно)
    /// </summary>
    public DateTime? InputExploitationDateTo { get; set; }

    /// <summary>
    /// Подбор оборудования для нового обслуживания
    /// </summary>
    public bool ForMaintenance { get; set; }

    /// <summary>
    /// Дата начала планируемого обслуживания
    /// </summary>
    public DateTime? MaintenanceBeginDate { get; set; }

    /// <summary>
    /// Дата окончания планируемого обслуживания
    /// </summary>
    public DateTime? MaintenanceEndDate { get; set; }

    /// <summary>
    /// Организация, которая проводит ТО
    /// </summary>
    public Guid? MaintenanceOrganizationId { get; set; }

    /// <summary>
    /// Сотрудник, который проводит ТО
    /// </summary>
    public Guid? MaintenanceEmployeeId { get; set; }
}
