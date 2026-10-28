using Contracts.Paging;
using FluentValidation;

namespace BL.Validation;

/// <summary>
/// Общие правила пагинации для всех списочных запросов
/// </summary>
/// <typeparam name="T">Тип параметров запроса</typeparam>
public abstract class BasePagingParametersValidator<T> : AbstractValidator<T> where T : PagingParameters
{
    protected BasePagingParametersValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(0, 300);

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0);
    }
}
