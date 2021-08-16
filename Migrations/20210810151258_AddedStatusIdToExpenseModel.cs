using Microsoft.EntityFrameworkCore.Migrations;

namespace GoldCap.Migrations
{
    public partial class AddedStatusIdToExpenseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Expenses");
        }
    }
}
