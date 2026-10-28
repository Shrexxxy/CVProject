using BL.Common;

namespace BL.Entities;

/// <summary>
/// Запись релейшена технического обслуживания
/// </summary>
public class MaintenanceEntity : DocumentIndexMetadata
{
    /// <summary>
    /// Организация, которой принадлежит оборудование
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Обслуживаемое оборудование
    /// </summary>
    public Guid[] EquipmentIds { get; set; } = [];

    /// <summary>
    /// Организация, которая проводит ТО
    /// </summary>
    public Guid? ContractorOrganizationId { get; set; }

    /// <summary>
    /// Сотрудник, который проводит ТО
    /// </summary>
    public Guid? ContractorEmployeeId { get; set; }

    /// <summary>
    /// Дата начала обслуживания
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// Дата окончания обслуживания
    /// </summary>
    public DateTime? EndDate { get; set; }
}
