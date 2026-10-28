namespace BL.Common;

/// <summary>
/// Настройки пагинации с сортировкой
/// </summary>
public class PagingSortSettings : PagingSettings
{
    /// <summary>
    /// Колонка для сортировки
    /// </summary>
    /// <example>null</example>
    public string SortColumn { get; set; } = string.Empty;

    /// <summary>
    /// Порядок сортировки
    /// </summary>
    public SortOrder SortOrder { get; set; } = SortOrder.Asc;
}
