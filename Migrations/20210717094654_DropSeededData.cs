using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoldCap.Migrations
{
    public partial class DropSeededData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Expenses",
                type: "nvarchar(135)",
                maxLength: 135,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Expenses",
                type: "smalldatetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(135)",
                oldMaxLength: 135,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Expenses",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "smalldatetime");

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "Category", "Date", "Description" },
                values: new object[,]
                {
                    { 1, 27m, 2, new DateTime(2021, 7, 16, 11, 48, 32, 413, DateTimeKind.Local).AddTicks(4174), "First decscription" },
                    { 16, 8005m, 3, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9601), "Fourth decscription" },
                    { 15, 35m, 7, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9597), "Third decscription" },
                    { 14, 14m, 6, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9594), "Second decscription" },
                    { 13, 27m, 2, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9591), "First decscription" },
                    { 12, 255m, 1, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9588), "Sixth decscription" },
                    { 11, 425m, 4, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9585), "Fifth decscription" },
                    { 10, 8005m, 3, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9582), "Very long description to check if everything is ok-Very long description to check if everything is ok-Ending of the sentence somewhere here should brk" },
                    { 9, 35m, 7, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9578), "Third decscription" },
                    { 8, 14m, 6, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9575), "Second decscription" },
                    { 7, 27m, 2, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9572), "First decscription" },
                    { 6, 255m, 1, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9568), "Sixth decscription" },
                    { 5, 425m, 4, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9565), "Fifth decscription" },
                    { 4, 8005m, 3, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9561), "Fourth decscription" },
                    { 3, 35m, 7, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9556), "Third decscription" },
                    { 2, 14m, 6, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9508), "Second decscription" },
                    { 17, 425m, 4, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9604), "Fifth decscription" },
                    { 18, 255m, 1, new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9607), "Sixth decscription" }
                });
        }
    }
}
