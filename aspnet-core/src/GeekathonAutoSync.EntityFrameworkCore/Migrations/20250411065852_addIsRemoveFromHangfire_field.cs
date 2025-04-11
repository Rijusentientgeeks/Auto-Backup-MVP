using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class addIsRemoveFromHangfire_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemoveFromHangfire",
                table: "BackUpSchedules",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoveFromHangfire",
                table: "BackUpSchedules");
        }
    }
}
