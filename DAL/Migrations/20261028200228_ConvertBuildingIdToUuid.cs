using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ConvertBuildingIdToUuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //сгенерированный AlterColumn<Guid> дропал ошибку, хотя запрос с кастом к uuid работал нормально, поэтому идём через новую колонку
            migrationBuilder.AddColumn<Guid>(
                name: "BuildingIdTemp",
                table: "EquipmentIndex",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""EquipmentIndex""
                SET ""BuildingIdTemp"" = NULLIF(""BuildingId"", '')::uuid
                WHERE ""BuildingId"" IS NOT NULL;");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "EquipmentIndex");

            migrationBuilder.RenameColumn(
                name: "BuildingIdTemp",
                table: "EquipmentIndex",
                newName: "BuildingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //откат кастомного решения в случае дропа миграции
            migrationBuilder.RenameColumn(
                name: "BuildingId",
                table: "EquipmentIndex",
                newName: "BuildingIdTemp");

            migrationBuilder.AddColumn<string>(
                name: "BuildingId",
                table: "EquipmentIndex",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""EquipmentIndex""
                SET ""BuildingId"" = ""BuildingIdTemp""::text
                WHERE ""BuildingIdTemp"" IS NOT NULL;");

            migrationBuilder.DropColumn(
                name: "BuildingIdTemp",
                table: "EquipmentIndex");
        }
    }
}
