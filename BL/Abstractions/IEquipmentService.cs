using Contracts;
using Contracts.Equipment;
using BL.Common;

namespace BL.Abstractions;

/// <summary>
/// Сервис оборудования
/// </summary>
public interface IEquipmentService
{
    /// <summary>
    /// Получить страницу списка оборудования по фильтру
    /// </summary>
    Task<PagingResult<EquipmentRecordItem>> GetItems(EquipmentFilterParameters filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить карточку оборудования
    /// </summary>
    Task<EquipmentModel?> GetById(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать карточку оборудования
    /// </summary>
    Task<DocumentMeta> Create(EquipmentParameters parameters);

    /// <summary>
    /// Изменить карточку оборудования
    /// </summary>
    Task<DocumentWriteResult> Update(Guid id, EquipmentUpdateParameters parameters);

    /// <summary>
    /// Мягкое удаление
    /// </summary>
    Task<DocumentWriteResult> Delete(Guid id);
}
