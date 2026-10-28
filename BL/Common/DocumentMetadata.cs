namespace BL.Common;

/// <summary>
/// Метаданные документа
/// </summary>
public class DocumentMetadata : DocumentIndexMetadata
{
    /// <summary>
    /// Дата и время последнего изменения, UTC
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Версия документа
    /// </summary>
    public int Version { get; set; }
}
