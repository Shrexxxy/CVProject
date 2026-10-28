namespace Contracts;

/// <summary>
/// Модель, которая хранится как версионируемый документ
/// </summary>
public interface IDocumentModel
{
    /// <summary>
    /// Идентификатор документа
    /// </summary>
    Guid EntityId { get; set; }

    /// <summary>
    /// Версия документа
    /// </summary>
    int Version { get; set; }

    /// <summary>
    /// Дата и время изменений
    /// </summary>
    DateTime Timestamp { get; set; }

    /// <summary>
    /// Версия схемы модели
    /// </summary>
    int ModelVersion { get; set; }
}
