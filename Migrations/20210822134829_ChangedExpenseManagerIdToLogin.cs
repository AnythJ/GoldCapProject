using Microsoft.EntityFrameworkCore.Migrations;

namespace GoldCap.Migrations
{
    public partial class ChangedExpenseManagerIdToLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseManagerId",
                table: "Expenses");

            migrationBuilder.AddColumn<string>(
                name: "ExpenseManagerLogin",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseManagerLogin",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "ExpenseManagerId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
