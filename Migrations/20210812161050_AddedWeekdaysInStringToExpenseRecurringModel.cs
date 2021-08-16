using Microsoft.EntityFrameworkCore.Migrations;

namespace GoldCap.Migrations
{
    public partial class AddedWeekdaysInStringToExpenseRecurringModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WeekdaysInString",
                table: "RecurringExpenses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekdaysInString",
                table: "RecurringExpenses");
        }
    }
}
