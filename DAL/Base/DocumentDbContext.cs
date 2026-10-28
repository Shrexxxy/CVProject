using BL.Common;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base;

/// <summary>
/// Контекст базы данных, в котором объявлены только документы
/// </summary>
/// <typeparam name="TDocument">Тип документа</typeparam>
public abstract class DocumentDbContext<TDocument>(DbContextOptions options) : DbContext(options)
    where TDocument : class
{
    /// <summary>
    /// Документы
    /// </summary>
    public DbSet<Document<TDocument>> Documents => Set<Document<TDocument>>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureDocumentEntity<TDocument>();
    }
}
