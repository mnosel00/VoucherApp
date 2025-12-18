using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VoucherApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedDel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RewardTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RewardTemplates",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RewardTemplates",
                columns: new[] { "Id", "BasketType", "BatchQuantity", "Category", "Name", "Value" },
                values: new object[,]
                {
                    { 1, 1, 8, 0, "Zniżka 10 PLN", 10m },
                    { 2, 1, 8, 1, "Zniżka 15%", 15m }
                });
        }
    }
}
