using Contracts.Equipment;
using FluentValidation;

namespace BL.Validation;

public class EquipmentParametersValidator : AbstractValidator<EquipmentParameters>
{
    private const int MaxNameLength = 500;
    private const int MaxNumberLength = 100;
    private const int MaxShortNameLength = 100;
    private const int MaxCount = 99_999;

    private static readonly DateTime MinDate = new(1900, 1, 1);

    private static readonly string[] EmptyNumberMarkers =
        ["б/н", "б\\н", "нет", "б/номера", "без номера", "отсутствует", "не предусмотрен", "0", "-"];

    private readonly DateTime _today = DateTime.UtcNow.Date;
    
    public EquipmentParametersValidator()
    {

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Не указана организация.");

        RuleFor(x => x.EquipmentType)
            .IsInEnum()
            .WithMessage("Указан некорректный вид оборудования.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Поле «Наименование» не должно быть пустым.")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Поле «Наименование» допускает не более {MaxNameLength} символов.");

        RuleFor(x => x.BuildingName)
            .MaximumLength(MaxNameLength)
            .WithMessage($"Поле «Здание» допускает не более {MaxNameLength} символов.");

        RuleFor(x => x.ManufacturerName)
            .NotEmpty()
            .WithMessage("Поле «Производитель» не должно быть пустым.")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Поле «Производитель» допускает не более {MaxNameLength} символов.");

        RuleFor(x => x.Model)
            .MaximumLength(MaxNameLength)
            .WithMessage($"Поле «Модель» допускает не более {MaxNameLength} символов.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty()
            .WithMessage("Поле «Серийный номер» не должно быть пустым.")
            .MaximumLength(MaxNumberLength)
            .WithMessage($"Поле «Серийный номер» допускает не более {MaxNumberLength} символов.");

        RuleFor(x => x.InventoryNumber)
            .NotEmpty()
            .WithMessage("Поле «Инвентарный номер» не должно быть пустым.")
            .MaximumLength(MaxNumberLength)
            .WithMessage($"Поле «Инвентарный номер» допускает не более {MaxNumberLength} символов.");

        When(x => x.EquipmentType != EquipmentType.Production, () =>
        {
            RuleFor(x => x.Model)
                .NotEmpty()
                .WithMessage("Поле «Модель» обязательно для непроизводственного оборудования.")
                .MaximumLength(MaxShortNameLength)
                .WithMessage($"Для непроизводственного оборудования поле «Модель» допускает не более {MaxShortNameLength} символов.");

            RuleFor(x => x.ManufacturerName)
                .MaximumLength(MaxShortNameLength)
                .WithMessage($"Для непроизводственного оборудования поле «Производитель» допускает не более {MaxShortNameLength} символов.");
        });

        #region Даты жизненного цикла

        RuleFor(x => x.ReleaseDateFormat)
            .Must(format => format is null or "y" or "m")
            .WithMessage("Точность даты выпуска может быть пустой, 'y' или 'm'.");

        RuleFor(x => x.ReleaseDate)
            .Must(date => date.Date >= MinDate)
            .WithMessage($"Поле «Дата выпуска» не может быть раньше {MinDate:dd.MM.yyyy}.")
            .Must(date => date.Date <= _today)
            .WithMessage($"Поле «Дата выпуска» не может быть позже {_today:dd.MM.yyyy}.")
            .Must((model, date) => model.ReleaseDateFormat switch
            {
                "m" => date.Day == 1,
                "y" => date is { Day: 1, Month: 1 },
                _ => true
            })
            .WithMessage("Дата выпуска не соответствует указанной точности: для месяца - первое число, для года - 1 января.");

        RuleFor(x => x.InputExploitationDate)
            .Must(date => date.Date >= MinDate)
            .WithMessage($"Поле «Дата ввода в эксплуатацию» не может быть раньше {MinDate:dd.MM.yyyy}.")
            .Must(date => date.Date <= _today)
            .WithMessage($"Поле «Дата ввода в эксплуатацию» не может быть позже {_today:dd.MM.yyyy}.")
            .Must((model, date) => date.Date >= model.ReleaseDate.Date)
            .WithMessage("Дата ввода в эксплуатацию не может быть раньше даты выпуска.");

        When(x => x.OutputExploitationDate.HasValue, () =>
            {
                RuleFor(x => x.ReasonOutputExploitation)
                    .NotEmpty()
                    .WithMessage("Укажите причину вывода из эксплуатации.");

                RuleFor(x => x.OutputExploitationDate!.Value)
                    .Must((model, date) => date.Date >= model.InputExploitationDate.Date)
                    .WithMessage("Дата вывода из эксплуатации не может быть раньше даты ввода в эксплуатацию.")
                    .Must(date => date.Date <= _today)
                    .WithMessage($"Поле «Дата вывода из эксплуатации» не может быть позже {_today:dd.MM.yyyy}.")
                    .OverridePropertyName(x => x.OutputExploitationDate);
            })
            .Otherwise(() =>
            {
                RuleFor(x => x.ReasonOutputExploitation)
                    .Empty()
                    .WithMessage("Причина вывода указывается только вместе с датой вывода из эксплуатации.");
            });

        #endregion

        When(x => IsEmptyNumber(x.SerialNumber) && IsEmptyNumber(x.InventoryNumber), () =>
            {
                RuleFor(x => x.Count)
                    .InclusiveBetween(1, MaxCount)
                    .WithMessage($"Поле «Количество» допускает значения от 1 до {MaxCount}.");
            })
            .Otherwise(() =>
            {
                RuleFor(x => x.Count)
                    .Equal(1)
                    .WithMessage("При указанных серийном и инвентарном номерах количество может быть только 1.");
            });
    }

    private static bool IsEmptyNumber(string? number)
    {
        return number is not null && EmptyNumberMarkers.Contains(number.Trim().ToLowerInvariant());
    }
}
