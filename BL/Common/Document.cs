namespace BL.Common;

/// <summary>
/// Модель документа, содержащая метаданные документа и его тело
/// </summary>
/// <typeparam name="T">Тип тела документа</typeparam>
public class Document<T> : DocumentMetadata where T : class
{
    /// <summary>
    /// Тело документа
    /// </summary>
    public T Model { get; set; } = null!;
}
