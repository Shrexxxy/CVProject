using AutoMapper;
using Contracts;
using Contracts.Equipment;
using BL.Abstractions;
using BL.Common;
using BL.Entities;

namespace BL.Services;

/// <inheritdoc/>
public class EquipmentService(IEquipmentRepository repository, IMapper mapper) : IEquipmentService
{
    /// <inheritdoc/>
    public Task<PagingResult<EquipmentRecordItem>> GetItems(EquipmentFilterParameters filter,
        CancellationToken cancellationToken = default)
    {
        var filterSettings = mapper.Map<EquipmentFilterSettings>(filter);

        return repository.GetItemsByFilter(filterSettings, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<EquipmentModel?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return repository.GetById(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DocumentMeta> Create(EquipmentParameters parameters)
    {
        var document = BuildDocument(Guid.NewGuid(), version: 1, parameters);
        await repository.Add(document);

        return new DocumentMeta(document.EntityId, document.Version);
    }

    /// <inheritdoc/>
    public async Task<DocumentWriteResult> Update(Guid id, EquipmentUpdateParameters parameters)
    {
        var metadata = await repository.GetDocumentMetadata(id);
        if (metadata is null || metadata.IsDeleted)
            return new DocumentWriteResult(DocumentWriteStatus.NotFound);

        if (metadata.Version != parameters.Version)
            return new DocumentWriteResult(DocumentWriteStatus.VersionConflict);

        var document = BuildDocument(id, parameters.Version + 1, parameters);
        var saved = await repository.Update(document, parameters.Version);

        return saved
            ? new DocumentWriteResult(DocumentWriteStatus.Ok, new DocumentMeta(id, document.Version))
            : new DocumentWriteResult(DocumentWriteStatus.VersionConflict);
    }

    /// <inheritdoc/>
    public async Task<DocumentWriteResult> Delete(Guid id)
    {
        var metadata = await repository.GetDocumentMetadata(id);
        if (metadata is null || metadata.IsDeleted)
            return new DocumentWriteResult(DocumentWriteStatus.NotFound);

        var archived = await repository.Archive(id, metadata.Version);

        return archived
            ? new DocumentWriteResult(DocumentWriteStatus.Ok, new DocumentMeta(id, metadata.Version + 1))
            : new DocumentWriteResult(DocumentWriteStatus.VersionConflict);
    }

    private Document<EquipmentModel> BuildDocument(Guid id, int version, EquipmentParameters parameters)
    {
        var timestamp = DateTime.UtcNow;

        var model = mapper.Map<EquipmentParameters, EquipmentModel>(parameters);
        model.EntityId = id;
        model.Version = version;
        model.Timestamp = timestamp;

        return new Document<EquipmentModel>
        {
            EntityId = id,
            Version = version,
            Timestamp = timestamp,
            Model = model
        };
    }
}
