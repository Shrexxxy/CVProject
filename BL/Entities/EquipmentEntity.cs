using Contracts.Equipment;
using Contracts;
using BL.Common;

namespace BL.Entities;

/// <summary>
/// Запись релейшена оборудования
/// </summary>
public class EquipmentEntity : DocumentIndexMetadata, IDraft
{
    /// <summary>
    /// Организация, которой принадлежит оборудование
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Тип оборудования
    /// </summary>
    public EquipmentType EquipmentType { get; set; }

    /// <summary>
    /// Идентификатор здания, в котором расположено оборудование
    /// </summary>
    public Guid? BuildingId { get; set; }

    /// <summary>
    /// Наименование здания, в котором расположено оборудование
    /// </summary>
    public string BuildingName { get; set; } = string.Empty;

    /// <summary>
    /// Наименование оборудования
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Наименование производителя
    /// </summary>
    public string ManufacturerName { get; set; } = string.Empty;

    /// <summary>
    /// Модель
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Серийный номер
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Инвентарный номер
    /// </summary>
    public string InventoryNumber { get; set; } = string.Empty;

    /// <summary>
    /// Дата выпуска
    /// </summary>
    public DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Дата ввода в эксплуатацию
    /// </summary>
    public DateTime InputExploitationDate { get; set; }

    /// <summary>
    /// Дата вывода из эксплуатации
    /// </summary>
    public DateTime? OutputExploitationDate { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Count { get; set; }

    /// <inheritdoc/>
    public bool IsDraft { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdateUtcDateTime { get; set; }
}
