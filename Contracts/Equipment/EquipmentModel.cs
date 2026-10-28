namespace Contracts.Equipment;

/// <summary>
/// Документ оборудования
/// </summary>
public class EquipmentModel : EquipmentParameters, IDocumentModel
{
    /// <inheritdoc/>
    public Guid EntityId { get; set; }

    /// <inheritdoc/>
    public int Version { get; set; }

    /// <inheritdoc/>
    public DateTime Timestamp { get; set; }

    /// <inheritdoc/>
    public int ModelVersion { get; set; }
}
