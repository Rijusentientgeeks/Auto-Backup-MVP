using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class CronExpressionAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CronExpression",
                table: "BackUpSchedules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CronExpression",
                table: "BackUpSchedules");
        }
    }
}
