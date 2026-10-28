namespace BL.Common;

/// <summary>
/// Итог записи документа
/// </summary>
public enum DocumentWriteStatus
{
    /// <summary>
    /// Документ сохранён
    /// </summary>
    Ok,

    /// <summary>
    /// Документ не найден или удалён
    /// </summary>
    NotFound,

    /// <summary>
    /// Документ уже изменён другим запросом
    /// </summary>
    VersionConflict
}
