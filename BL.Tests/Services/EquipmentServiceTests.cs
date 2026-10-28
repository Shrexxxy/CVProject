using AutoMapper;
using AwesomeAssertions;
using BL.Abstractions;
using BL.Common;
using BL.Mapping;
using BL.Services;
using Contracts.Equipment;
using Moq;
using NUnit.Framework;

namespace BL.Tests.Services;

[TestFixture]
public class EquipmentServiceTests
{
    private Mock<IEquipmentRepository> _repository = null!;
    private EquipmentService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EquipmentModelProfile>();
            cfg.AddProfile<EquipmentFilterProfile>();
        });

        _repository = new Mock<IEquipmentRepository>();
        _service = new EquipmentService(_repository.Object, configuration.CreateMapper());
    }

    #region Create

    [Test]
    public async Task Create_ShouldSaveFirstVersionOfDocument()
    {
        Document<EquipmentModel>? saved = null;
        _repository.Setup(x => x.Add(It.IsAny<Document<EquipmentModel>>()))
            .Callback((Document<EquipmentModel> document) => saved = document)
            .Returns(Task.CompletedTask);

        var result = await _service.Create(CreateParameters());

        saved.Should().NotBeNull();
        saved!.Version.Should().Be(1);
        saved.EntityId.Should().Be(result.EntityId);
        saved.Model.EntityId.Should().Be(result.EntityId);
        saved.Model.Version.Should().Be(1);
        saved.Model.Name.Should().Be("Насос погружной");
        result.Version.Should().Be(1);
    }

    #endregion

    #region Update

    [Test]
    public async Task Update_ShouldReturnNotFound_WhenDocumentIsDeleted()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetDocumentMetadata(id))
            .ReturnsAsync(new DocumentMetadata { EntityId = id, Version = 2, IsDeleted = true });

        var result = await _service.Update(id, CreateUpdateParameters(version: 2));

        result.Status.Should().Be(DocumentWriteStatus.NotFound);
        _repository.Verify(x => x.Update(It.IsAny<Document<EquipmentModel>>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task Update_ShouldReturnVersionConflict_WhenClientVersionIsOutdated()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetDocumentMetadata(id))
            .ReturnsAsync(new DocumentMetadata { EntityId = id, Version = 3 });

        var result = await _service.Update(id, CreateUpdateParameters(version: 2));

        result.Status.Should().Be(DocumentWriteStatus.VersionConflict);
        _repository.Verify(x => x.Update(It.IsAny<Document<EquipmentModel>>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task Update_ShouldSaveNextVersion_WhenVersionMatches()
    {
        var id = Guid.NewGuid();
        Document<EquipmentModel>? saved = null;
        _repository.Setup(x => x.GetDocumentMetadata(id))
            .ReturnsAsync(new DocumentMetadata { EntityId = id, Version = 2 });
        _repository.Setup(x => x.Update(It.IsAny<Document<EquipmentModel>>(), 2))
            .Callback((Document<EquipmentModel> document, int _) => saved = document)
            .ReturnsAsync(true);

        var result = await _service.Update(id, CreateUpdateParameters(version: 2));

        result.Status.Should().Be(DocumentWriteStatus.Ok);
        result.Document!.Version.Should().Be(3);
        saved!.EntityId.Should().Be(id);
        saved.Version.Should().Be(3);
        saved.Model.Version.Should().Be(3);
    }

    [Test]
    public async Task Update_ShouldReturnVersionConflict_WhenDocumentChangedConcurrently()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetDocumentMetadata(id))
            .ReturnsAsync(new DocumentMetadata { EntityId = id, Version = 2 });
        _repository.Setup(x => x.Update(It.IsAny<Document<EquipmentModel>>(), 2))
            .ReturnsAsync(false);

        var result = await _service.Update(id, CreateUpdateParameters(version: 2));

        result.Status.Should().Be(DocumentWriteStatus.VersionConflict);
        result.Document.Should().BeNull();
    }

    #endregion

    #region Delete

    [Test]
    public async Task Delete_ShouldArchiveWithCurrentVersion()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetDocumentMetadata(id))
            .ReturnsAsync(new DocumentMetadata { EntityId = id, Version = 4 });
        _repository.Setup(x => x.Archive(id, 4)).ReturnsAsync(true);

        var result = await _service.Delete(id);

        result.Status.Should().Be(DocumentWriteStatus.Ok);
        result.Document!.Version.Should().Be(5);
        _repository.Verify(x => x.Archive(id, 4), Times.Once);
    }

    [Test]
    public async Task Delete_ShouldReturnNotFound_WhenDocumentMissing()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetDocumentMetadata(id)).ReturnsAsync((DocumentMetadata?)null);

        var result = await _service.Delete(id);

        result.Status.Should().Be(DocumentWriteStatus.NotFound);
        _repository.Verify(x => x.Archive(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    #endregion

    private static EquipmentParameters CreateParameters()
    {
        return new EquipmentParameters
        {
            OrganizationId = Guid.NewGuid(),
            EquipmentType = EquipmentType.Production,
            Name = "Насос погружной",
            ManufacturerName = "Завод",
            SerialNumber = "SN-1",
            InventoryNumber = "INV-1",
            ReleaseDate = new DateTime(2020, 1, 1),
            InputExploitationDate = new DateTime(2020, 2, 1),
            Count = 1
        };
    }

    private static EquipmentUpdateParameters CreateUpdateParameters(int version)
    {
        return new EquipmentUpdateParameters
        {
            OrganizationId = Guid.NewGuid(),
            EquipmentType = EquipmentType.Production,
            Name = "Насос погружной",
            ManufacturerName = "Завод",
            SerialNumber = "SN-1",
            InventoryNumber = "INV-1",
            ReleaseDate = new DateTime(2020, 1, 1),
            InputExploitationDate = new DateTime(2020, 2, 1),
            Count = 1,
            Version = version
        };
    }
}
