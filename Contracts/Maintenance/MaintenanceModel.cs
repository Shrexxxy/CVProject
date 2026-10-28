namespace Contracts.Maintenance;

/// <summary>
/// Документ технического обслуживания оборудования
/// </summary>
public class MaintenanceModel : IDocumentModel
{
    /// <inheritdoc/>
    public Guid EntityId { get; set; }

    /// <inheritdoc/>
    public int Version { get; set; }

    /// <inheritdoc/>
    public DateTime Timestamp { get; set; }

    /// <inheritdoc/>
    public int ModelVersion { get; set; }

    /// <summary>
    /// Организация, которой принадлежит оборудование
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Обслуживаемое оборудование
    /// </summary>
    public List<Guid> EquipmentIds { get; set; } = [];

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
