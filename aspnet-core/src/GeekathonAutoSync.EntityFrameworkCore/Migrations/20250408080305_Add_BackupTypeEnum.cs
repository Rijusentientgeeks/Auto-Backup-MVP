using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class Add_BackupTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "StorageMasterTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "DBTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "CloudStorages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BackupTypeEnum",
                table: "BackUPTypes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "StorageMasterTypes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DBTypes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CloudStorages");

            migrationBuilder.DropColumn(
                name: "BackupTypeEnum",
                table: "BackUPTypes");
        }
    }
}
