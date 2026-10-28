using BL.Common;

namespace BL.Abstractions;

/// <summary>
/// Репозиторий для управления индексом документов и хранилищем документов
/// </summary>
/// <typeparam name="T">Тип тела документа</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Получить метаданные документа по идентификатору
    /// </summary>
    /// <param name="id">
    /// Идентификатор документа
    /// </param>
    /// <returns>
    /// Метаданные документа, если документ найден, <c>null</c> иначе
    /// </returns>
    Task<DocumentMetadata?> GetDocumentMetadata(Guid id);

    /// <summary>
    /// Добавить документ в хранилище документов и индекс документа в хранилище индекса
    /// </summary>
    /// <param name="document">
    /// Добавляемый документ
    /// </param>
    Task Add(Document<T> document);

    /// <summary>
    /// Обновить документ в хранилище документов и индекс документа в хранилище индекса
    /// </summary>
    /// <param name="document">
    /// Обновляемый документ с новой версией
    /// </param>
    /// <param name="expectedVersion">
    /// Версия, которая должна быть в хранилище на момент записи
    /// </param>
    /// <returns>
    /// <c>true</c>, если документ записан; <c>false</c>, если его уже изменил другой запрос
    /// </returns>
    Task<bool> Update(Document<T> document, int expectedVersion);

    /// <summary>
    /// Пометить удаленными документ в хранилище документов и индекс документа в хранилище индекса
    /// </summary>
    /// <param name="id">
    /// Идентификатор удаляемого документа
    /// </param>
    /// <param name="expectedVersion">
    /// Версия, которая должна быть в хранилище на момент удаления. Удаление увеличивает версию на 1
    /// </param>
    /// <returns>
    /// <c>true</c>, если документ помечен удалённым; <c>false</c>, если его уже изменил другой запрос
    /// </returns>
    Task<bool> Archive(Guid id, int expectedVersion);
}
