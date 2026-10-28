using Contracts;

namespace BL.Common;

/// <summary>
/// Результат записи документа
/// </summary>
/// <param name="Status">Итог записи</param>
/// <param name="Document">Идентификатор и версия, если документ сохранён</param>
public sealed record DocumentWriteResult(DocumentWriteStatus Status, DocumentMeta? Document = null);
