namespace Contracts;

/// <summary>
/// Модель, которая может находиться в статусе черновика
/// </summary>
public interface IDraft
{
    /// <summary>
    /// Показывает, является ли модель черновиком
    /// </summary>
    bool IsDraft { get; set; }
}
