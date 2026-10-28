using Contracts.Equipment;
using FluentValidation;

namespace BL.Validation;

public class EquipmentFilterParametersValidator : BasePagingParametersValidator<EquipmentFilterParameters>
{
    public EquipmentFilterParametersValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.InputExploitationDateTo)
            .GreaterThanOrEqualTo(x => x.InputExploitationDateFrom)
            .When(x => x.InputExploitationDateFrom.HasValue && x.InputExploitationDateTo.HasValue);

        When(x => x.ForMaintenance, () =>
        {
            RuleFor(x => x.MaintenanceBeginDate)
                .NotNull();

            RuleFor(x => x)
                .Must(x => x.MaintenanceOrganizationId.HasValue || x.MaintenanceEmployeeId.HasValue)
                .WithName("Maintenance")
                .WithMessage("Для подбора на обслуживание укажите организацию-подрядчика или сотрудника-исполнителя.");

            RuleFor(x => x.MaintenanceEndDate)
                .GreaterThanOrEqualTo(x => x.MaintenanceBeginDate)
                .When(x => x.MaintenanceBeginDate.HasValue && x.MaintenanceEndDate.HasValue);
        });
    }
}
