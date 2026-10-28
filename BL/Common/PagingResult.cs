namespace BL.Common;

/// <summary>
/// Представляет результат запроса с пагинацией
/// </summary>
/// <typeparam name="T">Тип данных</typeparam>
public sealed record PagingResult<T>
{
    /// <summary>
    /// Найденные записи
    /// </summary>
    public IReadOnlyCollection<T> Items { get; init; } = [];

    /// <summary>
    /// Общее количество записей
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// Представляет пустой результат
    /// </summary>
    public static PagingResult<T> Empty { get; } = new();
}
