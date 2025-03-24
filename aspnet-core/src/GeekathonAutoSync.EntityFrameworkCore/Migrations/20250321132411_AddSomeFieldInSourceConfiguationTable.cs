using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeFieldInSourceConfiguationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DatabaseName",
                table: "SourceConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DbPassword",
                table: "SourceConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DbUsername",
                table: "SourceConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Port",
                table: "SourceConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SshPassword",
                table: "SourceConfiguations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SshUserName",
                table: "SourceConfiguations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatabaseName",
                table: "SourceConfiguations");

            migrationBuilder.DropColumn(
                name: "DbPassword",
                table: "SourceConfiguations");

            migrationBuilder.DropColumn(
                name: "DbUsername",
                table: "SourceConfiguations");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "SourceConfiguations");

            migrationBuilder.DropColumn(
                name: "SshPassword",
                table: "SourceConfiguations");

            migrationBuilder.DropColumn(
                name: "SshUserName",
                table: "SourceConfiguations");
        }
    }
}
