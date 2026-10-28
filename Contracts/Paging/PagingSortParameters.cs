namespace Contracts.Paging;

/// <summary>
/// Параметры пагинации с сортировкой
/// </summary>
public class PagingSortParameters : PagingParameters
{
    /// <summary>
    /// Колонка для сортировки
    /// </summary>
    /// <example>null</example>
    public string? SortColumn { get; set; }

    /// <summary>
    /// Порядок сортировки. 0 - по возрастанию (ASC); 1 - по убыванию (DESC)
    /// </summary>
    public PagingSortOrder SortOrder { get; set; } = PagingSortOrder.Asc;
}
