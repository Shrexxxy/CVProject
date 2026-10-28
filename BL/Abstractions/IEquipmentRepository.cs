using Contracts.Equipment;
using BL.Common;
using BL.Entities;

namespace BL.Abstractions;

/// <summary>
/// Репозиторий оборудования
/// </summary>
public interface IEquipmentRepository : IRepository<EquipmentModel>
{
    /// <summary>
    /// Получить карточку оборудования по идентификатору
    /// </summary>
    Task<EquipmentModel?> GetById(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить страницу списка оборудования по фильтру
    /// </summary>
    Task<PagingResult<EquipmentRecordItem>> GetItemsByFilter(EquipmentFilterSettings filter,
        CancellationToken cancellationToken = default);
}
