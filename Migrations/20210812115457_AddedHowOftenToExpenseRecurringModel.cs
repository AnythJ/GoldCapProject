using Microsoft.EntityFrameworkCore.Migrations;

namespace GoldCap.Migrations
{
    public partial class AddedHowOftenToExpenseRecurringModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HowOften",
                table: "RecurringExpenses",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HowOften",
                table: "RecurringExpenses");
        }
    }
}
