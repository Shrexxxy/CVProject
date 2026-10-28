using AwesomeAssertions;
using BL.Validation;
using Contracts.Equipment;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace BL.Tests.Validation;

[TestFixture]
public class EquipmentParametersValidatorTests
{
    private EquipmentParametersValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new EquipmentParametersValidator();
    }

    [Test]
    public void Validate_ShouldPass_ForValidCard()
    {
        var result = _validator.TestValidate(CreateValid());

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Count_ShouldBeOne_WhenNumbersAreFilled()
    {
        var parameters = CreateValid();
        parameters.Count = 2;

        var result = _validator.TestValidate(parameters);

        result.ShouldHaveValidationErrorFor(x => x.Count);
    }

    [TestCase("б/н", "нет")]
    [TestCase("Б/Н", " - ")]
    [TestCase("без номера", "отсутствует")]
    public void Count_CanBeGreaterThanOne_WhenNumbersAreAbsent(string serialNumber, string inventoryNumber)
    {
        var parameters = CreateValid();
        parameters.SerialNumber = serialNumber;
        parameters.InventoryNumber = inventoryNumber;
        parameters.Count = 5;

        var result = _validator.TestValidate(parameters);

        result.ShouldNotHaveValidationErrorFor(x => x.Count);
    }

    [Test]
    public void ReleaseDate_ShouldBeFirstDayOfMonth_WhenFormatIsMonth()
    {
        var parameters = CreateValid();
        parameters.ReleaseDateFormat = "m";
        parameters.ReleaseDate = new DateTime(2020, 1, 15);

        var result = _validator.TestValidate(parameters);

        result.ShouldHaveValidationErrorFor(x => x.ReleaseDate);
    }

    [Test]
    public void ReleaseDate_ShouldNotBeInFuture()
    {
        var parameters = CreateValid();
        parameters.ReleaseDate = DateTime.UtcNow.Date.AddDays(1);
        parameters.InputExploitationDate = DateTime.UtcNow.Date.AddDays(1);

        var result = _validator.TestValidate(parameters);

        result.ShouldHaveValidationErrorFor(x => x.ReleaseDate);
    }

    [Test]
    public void InputExploitationDate_ShouldNotBeEarlierThanReleaseDate()
    {
        var parameters = CreateValid();
        parameters.InputExploitationDate = parameters.ReleaseDate.AddDays(-1);

        var result = _validator.TestValidate(parameters);

        result.ShouldHaveValidationErrorFor(x => x.InputExploitationDate);
    }

    [Test]
    public void ReasonOutputExploitation_ShouldBeRequired_WhenOutputDateFilled()
    {
        var parameters = CreateValid();
        parameters.OutputExploitationDate = new DateTime(2024, 1, 1);

        var result = _validator.TestValidate(parameters);

        result.ShouldHaveValidationErrorFor(x => x.ReasonOutputExploitation);
    }

    [Test]
    public void Model_ShouldBeRequired_ForNonProductionEquipment()
    {
        var parameters = CreateValid();
        parameters.EquipmentType = EquipmentType.Vehicle;
        parameters.Model = null;

        var result = _validator.TestValidate(parameters);

        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    private static EquipmentParameters CreateValid()
    {
        return new EquipmentParameters
        {
            OrganizationId = Guid.NewGuid(),
            EquipmentType = EquipmentType.Production,
            BuildingName = "Корпус 1",
            Name = "Насос погружной",
            ManufacturerName = "Завод",
            SerialNumber = "SN-1",
            InventoryNumber = "INV-1",
            ReleaseDate = new DateTime(2020, 1, 1),
            InputExploitationDate = new DateTime(2020, 2, 1),
            Count = 1
        };
    }
}
