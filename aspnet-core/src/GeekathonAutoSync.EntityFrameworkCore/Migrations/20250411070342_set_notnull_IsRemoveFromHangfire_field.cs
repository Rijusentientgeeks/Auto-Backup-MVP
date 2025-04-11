using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class set_notnull_IsRemoveFromHangfire_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsRemoveFromHangfire",
                table: "BackUpSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsRemoveFromHangfire",
                table: "BackUpSchedules",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
