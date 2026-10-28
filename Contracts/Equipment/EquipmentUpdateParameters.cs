namespace Contracts.Equipment;

/// <summary>
/// Параметры изменения единицы оборудования
/// </summary>
public class EquipmentUpdateParameters : EquipmentParameters
{
    /// <summary>
    /// Версия документа
    /// </summary>
    public int Version { get; set; }
}
