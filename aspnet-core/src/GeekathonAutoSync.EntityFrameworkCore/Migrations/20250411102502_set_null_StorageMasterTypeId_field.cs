using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class set_null_StorageMasterTypeId_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackUpStorageConfiguations_StorageMasterTypes_StorageMasterTypeId",
                table: "BackUpStorageConfiguations");

            migrationBuilder.AlterColumn<Guid>(
                name: "StorageMasterTypeId",
                table: "BackUpStorageConfiguations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_BackUpStorageConfiguations_StorageMasterTypes_StorageMasterTypeId",
                table: "BackUpStorageConfiguations",
                column: "StorageMasterTypeId",
                principalTable: "StorageMasterTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackUpStorageConfiguations_StorageMasterTypes_StorageMasterTypeId",
                table: "BackUpStorageConfiguations");

            migrationBuilder.AlterColumn<Guid>(
                name: "StorageMasterTypeId",
                table: "BackUpStorageConfiguations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BackUpStorageConfiguations_StorageMasterTypes_StorageMasterTypeId",
                table: "BackUpStorageConfiguations",
                column: "StorageMasterTypeId",
                principalTable: "StorageMasterTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
