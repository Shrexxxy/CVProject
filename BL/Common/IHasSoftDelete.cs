namespace BL.Common;

/// <summary>
/// Сущность с мягким удалением: запись не удаляется физически, а помечается флагом
/// </summary>
public interface IHasSoftDelete
{
    /// <summary>
    /// Признак, указывающий, что документ помечен как удалённый
    /// </summary>
    bool IsDeleted { get; }
}
