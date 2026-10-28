namespace Contracts.Equipment;

/// <summary>
/// Строка списка оборудования
/// </summary>
public sealed record EquipmentRecordItem
{
    /// <summary>
    /// Идентификатор оборудования
    /// </summary>
    public Guid EntityId { get; init; }
    
    /// <summary>
    /// Организация, которой принадлежит оборудование
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Тип оборудования
    /// </summary>
    public EquipmentType EquipmentType { get; init; }

    /// <summary>
    /// Наименование здания
    /// </summary>
    public string BuildingName { get; init; } = string.Empty;

    /// <summary>
    /// Наименование оборудования
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Наименование производителя
    /// </summary>
    public string ManufacturerName { get; init; } = string.Empty;

    /// <summary>
    /// Модель
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Серийный номер
    /// </summary>
    public string SerialNumber { get; init; } = string.Empty;

    /// <summary>
    /// Дата выпуска
    /// </summary>
    public DateTime ReleaseDate { get; init; }

    /// <summary>
    /// Дата ввода в эксплуатацию
    /// </summary>
    public DateTime InputExploitationDate { get; init; }

    /// <summary>
    /// Дата вывода из эксплуатации
    /// </summary>
    public DateTime? OutputExploitationDate { get; init; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Показывает, является ли запись черновиком
    /// </summary>
    public bool IsDraft { get; init; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdateDate { get; init; }
}
