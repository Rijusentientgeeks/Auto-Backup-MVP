using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class BackUPName_Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackUpStorageConfiguations_AbpTenants_TenantId",
                table: "BackUpStorageConfiguations");

            migrationBuilder.AddColumn<string>(
                name: "BackupName",
                table: "SourceConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "BackUpStorageConfiguations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BackupName",
                table: "BackUpStorageConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BackUpStorageConfiguations_AbpTenants_TenantId",
                table: "BackUpStorageConfiguations",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackUpStorageConfiguations_AbpTenants_TenantId",
                table: "BackUpStorageConfiguations");

            migrationBuilder.DropColumn(
                name: "BackupName",
                table: "SourceConfiguations");

            migrationBuilder.DropColumn(
                name: "BackupName",
                table: "BackUpStorageConfiguations");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "BackUpStorageConfiguations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BackUpStorageConfiguations_AbpTenants_TenantId",
                table: "BackUpStorageConfiguations",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
