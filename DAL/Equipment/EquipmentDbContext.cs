using Contracts.Equipment;
using Contracts.Maintenance;
using BL.Common;
using BL.Entities;
using Microsoft.EntityFrameworkCore;
using DAL.Base;

namespace DAL.Equipment;

/// <summary>
/// Контекст оборудования и его технического обслуживания
/// </summary>
public class EquipmentDbContext(DbContextOptions<EquipmentDbContext> options)
    : DocumentIndexDbContext<EquipmentModel, EquipmentEntity>(options)
{
    /// <summary>
    /// Документы обслуживания
    /// </summary>
    public DbSet<Document<MaintenanceModel>> MaintenanceDocuments => Set<Document<MaintenanceModel>>();

    /// <summary>
    /// Релейшен обслуживания
    /// </summary>
    public DbSet<MaintenanceEntity> MaintenanceDocumentIndex => Set<MaintenanceEntity>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.Entity<Document<EquipmentModel>>().ToTable("EquipmentDocuments");

        #region Equipment

        modelBuilder.Entity<EquipmentEntity>(e =>
        {
            e.ToTable("EquipmentIndex");

            e.HasIndex(x => new { x.OrganizationId, x.UpdateUtcDateTime })
                .HasFilter("\"IsDeleted\" = false")
                .HasDatabaseName("IX_EquipmentIndex_OrganizationId_UpdateUtcDateTime");

            e.HasIndex(x => x.Name)
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops")
                .HasDatabaseName("IX_EquipmentIndex_Name_Trgm");

            e.HasIndex(x => x.Model)
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops")
                .HasDatabaseName("IX_EquipmentIndex_Model_Trgm");

            e.Property(x => x.Name).HasMaxLength(500);
            e.Property(x => x.BuildingName).HasMaxLength(500);
            e.Property(x => x.ManufacturerName).HasMaxLength(500);
            e.Property(x => x.Model).HasMaxLength(500);
            e.Property(x => x.SerialNumber).HasMaxLength(100);
            e.Property(x => x.InventoryNumber).HasMaxLength(100);

            e.Property(x => x.ReleaseDate).HasColumnType("date");
            e.Property(x => x.InputExploitationDate).HasColumnType("date");
            e.Property(x => x.OutputExploitationDate).HasColumnType("date");
        });

        #endregion

        #region Maintenance

        modelBuilder.ConfigureDocumentEntity<MaintenanceModel>();
        modelBuilder.Entity<Document<MaintenanceModel>>().ToTable("MaintenanceDocuments");

        modelBuilder.ConfigureDocumentIndexEntity<MaintenanceEntity>();

        modelBuilder.Entity<MaintenanceEntity>(e =>
        {
            e.ToTable("MaintenanceIndex");

            e.Property(x => x.BeginDate).HasColumnType("date");
            e.Property(x => x.EndDate).HasColumnType("date");

            e.HasIndex(x => x.EquipmentIds)
                .HasMethod("gin")
                .HasDatabaseName("IX_MaintenanceIndex_EquipmentIds_Gin");

            e.HasIndex(x => new { x.ContractorOrganizationId, x.BeginDate })
                .HasFilter("\"IsDeleted\" = false AND \"ContractorOrganizationId\" IS NOT NULL")
                .HasDatabaseName("IX_MaintenanceIndex_ContractorOrganizationId_BeginDate");

            e.HasIndex(x => new { x.ContractorEmployeeId, x.BeginDate })
                .HasFilter("\"IsDeleted\" = false AND \"ContractorEmployeeId\" IS NOT NULL")
                .HasDatabaseName("IX_MaintenanceIndex_ContractorEmployeeId_BeginDate");
        });

        #endregion
    }
}
