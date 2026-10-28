namespace BL.Common;

/// <summary>
/// Метаданные индекса документа
/// </summary>
public class DocumentIndexMetadata : IHasSoftDelete
{
    /// <summary>
    /// Идентификатор документа
    /// </summary>
    public Guid EntityId { get; set; }

    /// <inheritdoc/>
    public bool IsDeleted { get; set; }
}
