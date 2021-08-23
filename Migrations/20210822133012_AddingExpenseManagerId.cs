using Microsoft.EntityFrameworkCore.Migrations;

namespace GoldCap.Migrations
{
    public partial class AddingExpenseManagerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpenseManagerId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseManagerId",
                table: "AspNetUsers");
        }
    }
}
