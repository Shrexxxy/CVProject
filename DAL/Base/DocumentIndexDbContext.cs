using BL.Common;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base;

/// <summary>
/// Контекст базы данных, в котором объявлены документ и индекс документов
/// </summary>
/// <typeparam name="TDocument">Тип документа</typeparam>
/// <typeparam name="TEntity">Тип индекса</typeparam>
public abstract class DocumentIndexDbContext<TDocument, TEntity>(DbContextOptions options)
    : DocumentDbContext<TDocument>(options)
    where TDocument : class
    where TEntity : DocumentIndexMetadata
{
    /// <summary>
    /// Релейшен документов
    /// </summary>
    public DbSet<TEntity> DocumentIndex => Set<TEntity>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureDocumentIndexEntity<TEntity>();
    }
}
