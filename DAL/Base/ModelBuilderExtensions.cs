using BL.Common;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base;

/// <summary>
/// Общая конфигурация документов и релейшенов
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Добавить конфигурацию для документов: тело документа хранится в jsonb
    /// </summary>
    /// <typeparam name="TDocument">
    /// Тип документа
    /// </typeparam>
    /// <param name="modelBuilder">Построитель модели</param>
    public static void ConfigureDocumentEntity<TDocument>(this ModelBuilder modelBuilder)
        where TDocument : class
    {
        modelBuilder.Entity<Document<TDocument>>(e =>
        {
            e.HasKey(x => x.EntityId);
            e.Property(x => x.EntityId).ValueGeneratedNever();
            e.Property(x => x.Model).HasColumnType("jsonb");
            e.Property(x => x.Version).IsConcurrencyToken();
        });
    }

    /// <summary>
    /// Добавить конфигурацию для списочного представления документов
    /// </summary>
    /// <typeparam name="TEntity">
    /// Тип списочного представления
    /// </typeparam>
    /// <param name="modelBuilder">Построитель модели</param>
    /// <returns>
    /// Переданный <see cref="ModelBuilder"/> (для построения цепочки вызовов)
    /// </returns>
    public static ModelBuilder ConfigureDocumentIndexEntity<TEntity>(this ModelBuilder modelBuilder)
        where TEntity : DocumentIndexMetadata
    {
        modelBuilder.Entity<TEntity>(e =>
        {
            e.HasKey(x => x.EntityId);
            e.Property(x => x.EntityId).ValueGeneratedNever();
        });

        return modelBuilder;
    }
}
