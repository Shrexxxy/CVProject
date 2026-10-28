namespace Contracts;

/// <summary>
/// Идентификатор и версия сохранённого документа
/// </summary>
/// <param name="EntityId">Идентификатор документа</param>
/// <param name="Version">Версия документа после сохранения</param>
public sealed record DocumentMeta(Guid EntityId, int Version);
