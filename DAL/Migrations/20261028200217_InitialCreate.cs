using System;
using Contracts.Equipment;
using Contracts.Maintenance;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "EquipmentDocuments",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<EquipmentModel>(type: "jsonb", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDocuments", x => x.EntityId);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentIndex",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentType = table.Column<int>(type: "integer", nullable: false),
                    BuildingId = table.Column<string>(type: "text", nullable: true),
                    BuildingName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ManufacturerName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Model = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InventoryNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "date", nullable: false),
                    InputExploitationDate = table.Column<DateTime>(type: "date", nullable: false),
                    OutputExploitationDate = table.Column<DateTime>(type: "date", nullable: true),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateUtcDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentIndex", x => x.EntityId);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceDocuments",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<MaintenanceModel>(type: "jsonb", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceDocuments", x => x.EntityId);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceIndex",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    ContractorOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContractorEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceIndex", x => x.EntityId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentIndex_Model_Trgm",
                table: "EquipmentIndex",
                column: "Model")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentIndex_Name_Trgm",
                table: "EquipmentIndex",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentIndex_OrganizationId_UpdateUtcDateTime",
                table: "EquipmentIndex",
                columns: new[] { "OrganizationId", "UpdateUtcDateTime" },
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceIndex_ContractorEmployeeId_BeginDate",
                table: "MaintenanceIndex",
                columns: new[] { "ContractorEmployeeId", "BeginDate" },
                filter: "\"IsDeleted\" = false AND \"ContractorEmployeeId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceIndex_ContractorOrganizationId_BeginDate",
                table: "MaintenanceIndex",
                columns: new[] { "ContractorOrganizationId", "BeginDate" },
                filter: "\"IsDeleted\" = false AND \"ContractorOrganizationId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceIndex_EquipmentIds_Gin",
                table: "MaintenanceIndex",
                column: "EquipmentIds")
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentDocuments");

            migrationBuilder.DropTable(
                name: "EquipmentIndex");

            migrationBuilder.DropTable(
                name: "MaintenanceDocuments");

            migrationBuilder.DropTable(
                name: "MaintenanceIndex");
        }
    }
}
