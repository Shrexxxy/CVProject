using Contracts.Equipment;
using FluentValidation;

namespace BL.Validation;

public class EquipmentUpdateParametersValidator : AbstractValidator<EquipmentUpdateParameters>
{
    public EquipmentUpdateParametersValidator(IValidator<EquipmentParameters> parametersValidator)
    {
        Include(parametersValidator);

        RuleFor(x => x.Version)
            .GreaterThan(0)
            .WithMessage("Не указана версия изменяемого документа.");
    }
}
