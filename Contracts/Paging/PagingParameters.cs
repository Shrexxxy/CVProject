namespace Contracts.Paging;

/// <summary>
/// Параметры пагинации
/// </summary>
public class PagingParameters
{
    /// <summary>
    /// Количество записей на странице по умолчанию
    /// </summary>
    private const int DefaultLimit = 15;

    /// <summary>
    /// Максимальное количество записей, которое должен вернуть запрос
    /// </summary>
    /// <example>15</example>
    public int Limit { get; set; } = DefaultLimit;

    /// <summary>
    /// Смещение
    /// </summary>
    public int Offset { get; set; }
}
