using AutoMapper;
using Contracts;
using BL.Abstractions;
using BL.Common;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base;

/// <summary>
/// Базовый репозиторий
/// </summary>
/// <typeparam name="TDocument">Тип тела документа</typeparam>
/// <typeparam name="TEntity">Тип записи релейшена</typeparam>
public abstract class RepositoryBase<TDocument, TEntity>(DbContext dbContext, IMapper mapper)
    : IRepository<TDocument>
    where TDocument : class, IDocumentModel
    where TEntity : DocumentIndexMetadata
{
    private readonly DbSet<Document<TDocument>> _documents = dbContext.Set<Document<TDocument>>();
    private readonly DbSet<TEntity> _documentIndex = dbContext.Set<TEntity>();

    /// <summary>
    /// Маппер документа в запись релейшена
    /// </summary>
    protected IMapper Mapper { get; } = mapper;

    /// <inheritdoc/>
    public Task<DocumentMetadata?> GetDocumentMetadata(Guid id)
    {
        return _documents
            .Where(x => x.EntityId == id)
            .Select(x => new DocumentMetadata
            {
                EntityId = x.EntityId,
                Timestamp = x.Timestamp,
                Version = x.Version,
                IsDeleted = x.IsDeleted
            })
            .SingleOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task Add(Document<TDocument> document)
    {
        AddDocument(document);
        await CreateIndex(document);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<bool> Update(Document<TDocument> document, int expectedVersion)
    {
        UpdateDocument(document, expectedVersion);
        await UpdateIndex(document);

        return await TrySaveChanges();
    }

    /// <inheritdoc/>
    public async Task<bool> Archive(Guid id, int expectedVersion)
    {
        ArchiveDocument(id, expectedVersion);
        await ArchiveIndex(id);

        return await TrySaveChanges();
    }

    /// <summary>
    /// Добавить документ в контекст
    /// </summary>
    /// <param name="document">Документ</param>
    protected virtual void AddDocument(Document<TDocument> document)
    {
        _documents.Add(document);
    }

    /// <summary>
    /// Пометить документ изменённым целиком, без предварительного чтения
    /// </summary>
    /// <param name="document">Документ</param>
    /// <param name="expectedVersion">Версия, которая должна быть в хранилище</param>
    protected virtual void UpdateDocument(Document<TDocument> document, int expectedVersion)
    {
        var entry = dbContext.Attach(document);
        entry.State = EntityState.Modified;

        entry.Property(x => x.Version).OriginalValue = expectedVersion;
    }

    /// <summary>
    /// Пометить документ удалённым и увеличить версию, без предварительного чтения
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    /// <param name="expectedVersion">Версия, которая должна быть в хранилище</param>
    protected virtual void ArchiveDocument(Guid id, int expectedVersion)
    {
        var document = new Document<TDocument> { EntityId = id, Version = expectedVersion };
        dbContext.Attach(document);
        document.Version = expectedVersion + 1;
        document.IsDeleted = true;
    }

    /// <summary>
    /// Создать запись релейшена по документу
    /// </summary>
    /// <param name="document">Документ</param>
    protected virtual Task CreateIndex(Document<TDocument> document)
    {
        var entity = Mapper.Map<TEntity>(document.Model);
        entity.EntityId = document.EntityId;
        entity.IsDeleted = false;

        _documentIndex.Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Обновить запись релейшена по документу
    /// </summary>
    /// <param name="document">Документ</param>
    protected virtual async Task UpdateIndex(Document<TDocument> document)
    {
        var entity = await _documentIndex.SingleAsync(x => x.EntityId == document.EntityId);
        Mapper.Map(document.Model, entity);
    }

    /// <summary>
    /// Пометить запись релейшена удалённой
    /// </summary>
    /// <param name="id">Идентификатор документа</param>
    protected virtual async Task ArchiveIndex(Guid id)
    {
        var entity = await _documentIndex.SingleAsync(x => x.EntityId == id);
        entity.IsDeleted = true;
    }

    /// <summary>
    /// Сохранить изменения
    /// </summary>
    private async Task<bool> TrySaveChanges()
    {
        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }
}
