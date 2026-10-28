using System.Linq.Expressions;
using BL.Common;
using BL.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using DAL.Base;

namespace DAL.Equipment;

/// <inheritdoc/>
public class EquipmentQueryBuilder : IQueryBuilder<EquipmentEntity, EquipmentFilterSettings>
{
    private const string LikeEscape = "\\";

    /// <inheritdoc/>
    public Expression<Func<EquipmentEntity, bool>> BuildQueryFilter(EquipmentFilterSettings filter)
    {
        var organizationId = filter.OrganizationId;
        var predicate = PredicateBuilder.New<EquipmentEntity>(x => !x.IsDeleted && x.OrganizationId == organizationId);

        if (filter.BuildingIds.Count > 0)
        {
            var buildingIds = filter.BuildingIds;
            predicate = predicate.And(x => x.BuildingId.HasValue && buildingIds.Contains(x.BuildingId.Value));
        }

        if (filter.EquipmentTypes.Count > 0)
        {
            var equipmentTypes = filter.EquipmentTypes;
            predicate = predicate.And(x => equipmentTypes.Contains(x.EquipmentType));
        }

        if (!filter.IncludeExploitationEnded)
        {
            var today = DateTime.UtcNow.Date;
            predicate = predicate.And(x => x.OutputExploitationDate == null || x.OutputExploitationDate > today);
        }

        if (!filter.IncludeDraft)
            predicate = predicate.And(x => !x.IsDraft);

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var pattern = ContainsPattern(filter.Name);
            predicate = predicate.And(x => EF.Functions.ILike(x.Name, pattern, LikeEscape));
        }

        if (!string.IsNullOrWhiteSpace(filter.ManufacturerName))
        {
            var pattern = ContainsPattern(filter.ManufacturerName);
            predicate = predicate.And(x => EF.Functions.ILike(x.ManufacturerName, pattern, LikeEscape));
        }

        if (!string.IsNullOrWhiteSpace(filter.Model))
        {
            var pattern = ContainsPattern(filter.Model);
            predicate = predicate.And(x => x.Model != null && EF.Functions.ILike(x.Model, pattern, LikeEscape));
        }

        if (!string.IsNullOrWhiteSpace(filter.SerialNumber))
        {
            var serialNumber = filter.SerialNumber.Trim();
            predicate = predicate.And(x => x.SerialNumber == serialNumber);
        }

        if (!string.IsNullOrWhiteSpace(filter.InventoryNumber))
        {
            var inventoryNumber = filter.InventoryNumber.Trim();
            predicate = predicate.And(x => x.InventoryNumber == inventoryNumber);
        }

        if (filter.InputExploitationDateFrom.HasValue)
        {
            var from = filter.InputExploitationDateFrom.Value.Date;
            predicate = predicate.And(x => x.InputExploitationDate >= from);
        }

        if (filter.InputExploitationDateTo.HasValue)
        {
            var to = filter.InputExploitationDateTo.Value.Date;
            predicate = predicate.And(x => x.InputExploitationDate <= to);
        }

        return predicate;
    }

    /// <inheritdoc/>
    public IOrderedQueryable<EquipmentEntity> ApplySortSettings(IQueryable<EquipmentEntity> query,
        PagingSortSettings pagingSettings)
    {
        var descending = pagingSettings.SortOrder == SortOrder.Desc;

        var ordered = pagingSettings.SortColumn.ToLowerInvariant() switch
        {
            "buildingname" => OrderBy(query, x => x.BuildingName, descending),
            "name" => OrderBy(query, x => x.Name, descending),
            "manufacturername" => OrderBy(query, x => x.ManufacturerName, descending),
            "releasedate" => OrderBy(query, x => x.ReleaseDate, descending),
            "inputexploitationdate" => OrderBy(query, x => x.InputExploitationDate, descending),
            "count" => OrderBy(query, x => x.Count, descending),
            _ => OrderBy(query, x => x.UpdateUtcDateTime, descending)
        };

        return ordered.ThenBy(x => x.EntityId);
    }

    private static IOrderedQueryable<EquipmentEntity> OrderBy<TKey>(IQueryable<EquipmentEntity> query,
        Expression<Func<EquipmentEntity, TKey>> keySelector, bool descending)
    {
        return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }

    /// <summary>
    /// Поиск по подстроке
    /// </summary>
    private static string ContainsPattern(string value)
    {
        var escaped = value.Trim()
            .Replace(LikeEscape, LikeEscape + LikeEscape)
            .Replace("%", LikeEscape + "%")
            .Replace("_", LikeEscape + "_");

        return $"%{escaped}%";
    }
}
