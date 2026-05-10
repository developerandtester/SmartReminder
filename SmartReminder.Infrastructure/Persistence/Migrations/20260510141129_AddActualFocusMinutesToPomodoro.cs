using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartReminder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddActualFocusMinutesToPomodoro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActualFocusMinutes",
                table: "PomodoroSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualFocusMinutes",
                table: "PomodoroSessions");
        }
    }
}
