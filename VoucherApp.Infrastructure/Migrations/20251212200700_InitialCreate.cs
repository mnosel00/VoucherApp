using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VoucherApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RewardTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasketType = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BatchQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RewardTemplateId = table.Column<int>(type: "int", nullable: false),
                    QrCodeContent = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRedeemed = table.Column<bool>(type: "bit", nullable: false),
                    RedeemedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vouchers_RewardTemplates_RewardTemplateId",
                        column: x => x.RewardTemplateId,
                        principalTable: "RewardTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RewardTemplates",
                columns: new[] { "Id", "BasketType", "BatchQuantity", "Category", "Name", "Value" },
                values: new object[,]
                {
                    { 1, 1, 8, 0, "Zniżka 10 PLN", 10m },
                    { 2, 1, 8, 1, "Zniżka 15%", 15m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_QrCodeContent",
                table: "Vouchers",
                column: "QrCodeContent",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_RewardTemplateId",
                table: "Vouchers",
                column: "RewardTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_ShortCode",
                table: "Vouchers",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "RewardTemplates");
        }
    }
}
