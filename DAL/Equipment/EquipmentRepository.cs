using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts.Equipment;
using BL.Abstractions;
using BL.Common;
using BL.Entities;
using Microsoft.EntityFrameworkCore;
using DAL.Base;

namespace DAL.Equipment;

/// <inheritdoc cref="IEquipmentRepository"/>
public class EquipmentRepository(
    EquipmentDbContext dbContext,
    IQueryBuilder<EquipmentEntity, EquipmentFilterSettings> queryBuilder,
    IMapper mapper)
    : RepositoryBase<EquipmentModel, EquipmentEntity>(dbContext, mapper), IEquipmentRepository
{
    /// <inheritdoc/>
    public Task<EquipmentModel?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Documents
            .AsNoTracking()
            .Where(x => x.EntityId == id && !x.IsDeleted)
            .Select(x => x.Model)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PagingResult<EquipmentRecordItem>> GetItemsByFilter(EquipmentFilterSettings filter,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.DocumentIndex
            .AsNoTracking()
            .Where(queryBuilder.BuildQueryFilter(filter));

        if (filter is { ForMaintenance: true, MaintenanceBeginDate: not null }
            && (filter.MaintenanceOrganizationId.HasValue || filter.MaintenanceEmployeeId.HasValue))
        {
            // Фильтрация по датам и организациям/персоналу обслуживания не может быть выполнена на уровне предиката,
            // так как требует доступа к другому DbSet и сложному подзапросу
            query = ExcludeBusyWithMaintenance(query, filter);
        }

        var total = await query.CountAsync(cancellationToken);
        if (total == 0)
            return PagingResult<EquipmentRecordItem>.Empty;

        if (filter.Limit == 0 || filter.Offset >= total)
            return new PagingResult<EquipmentRecordItem> { Total = total };

        var items = await queryBuilder.ApplySortSettings(query, filter)
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .ProjectTo<EquipmentRecordItem>(Mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);

        return new PagingResult<EquipmentRecordItem>
        {
            Items = items,
            Total = total
        };
    }

    /// <summary>
    /// Исключить оборудование, которое у того же исполнителя уже обслуживается в пересекающийся период
    /// </summary>
    private IQueryable<EquipmentEntity> ExcludeBusyWithMaintenance(IQueryable<EquipmentEntity> query,
        EquipmentFilterSettings filter)
    {
        var begin = filter.MaintenanceBeginDate!.Value.Date;
        var organizationId = filter.MaintenanceOrganizationId;
        var employeeId = filter.MaintenanceEmployeeId;

        var busy = dbContext.MaintenanceDocumentIndex
            .Where(m => !m.IsDeleted && (m.EndDate == null || m.EndDate >= begin));

        if (filter.MaintenanceEndDate.HasValue)
        {
            var end = filter.MaintenanceEndDate.Value.Date;
            busy = busy.Where(m => m.BeginDate <= end);
        }

        busy = (organizationId, employeeId) switch
        {
            ({ } orgId, { } empId) => busy.Where(m => m.ContractorOrganizationId == orgId || m.ContractorEmployeeId == empId),
            ({ } orgId, null) => busy.Where(m => m.ContractorOrganizationId == orgId),
            (null, { } empId) => busy.Where(m => m.ContractorEmployeeId == empId),
            _ => busy
        };

        return query.Where(x => !busy.Any(m => m.EquipmentIds.Contains(x.EntityId)));
    }
}
