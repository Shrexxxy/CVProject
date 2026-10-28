using System.Linq.Expressions;
using BL.Common;

namespace DAL.Base;

/// <summary>
/// Построитель запроса к индексу документов: фильтрация и сортировка
/// </summary>
/// <typeparam name="TEntity">Тип индексной записи</typeparam>
/// <typeparam name="TFilter">Тип фильтра</typeparam>
public interface IQueryBuilder<TEntity, in TFilter>
    where TEntity : class
    where TFilter : PagingSortSettings
{
    /// <summary>
    /// Построить предикат по фильтру
    /// </summary>
    /// <param name="filter">Фильтр</param>
    Expression<Func<TEntity, bool>> BuildQueryFilter(TFilter filter);

    /// <summary>
    /// Применить сортировку. Порядок всегда детерминирован, чтобы страницы не пересекались
    /// </summary>
    /// <param name="query">Исходный запрос</param>
    /// <param name="pagingSettings">Настройки сортировки</param>
    IOrderedQueryable<TEntity> ApplySortSettings(IQueryable<TEntity> query, PagingSortSettings pagingSettings);
}
