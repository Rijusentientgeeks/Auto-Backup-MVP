using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class modify_backupstorageconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CredentialFile",
                table: "BackUpStorageConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "BackUpStorageConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectID",
                table: "BackUpStorageConfiguations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CredentialFile",
                table: "BackUpStorageConfiguations");

            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "BackUpStorageConfiguations");

            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "BackUpStorageConfiguations");
        }
    }
}
