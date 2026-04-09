using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValueCo.CashUpApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                });

            migrationBuilder.CreateTable(
                name: "CashUpDays",
                columns: table => new
                {
                    DayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CashUpDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FNB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Surrender = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Floats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChangeBoxes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LooseChange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PettyCash = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PettyNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashUpNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountingBanking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Submitted = table.Column<bool>(type: "bit", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashUpDays", x => x.DayId);
                    table.ForeignKey(
                        name: "FK_CashUpDays_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashierRows",
                columns: table => new
                {
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayId = table.Column<int>(type: "int", nullable: false),
                    CashierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cash = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Card = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Erase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Returns = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gift = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Coupon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Loyalty = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashierRows", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_CashierRows_CashUpDays_DayId",
                        column: x => x.DayId,
                        principalTable: "CashUpDays",
                        principalColumn: "DayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EFTDetails",
                columns: table => new
                {
                    EFTId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayId = table.Column<int>(type: "int", nullable: false),
                    EFTDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SONumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFTDetails", x => x.EFTId);
                    table.ForeignKey(
                        name: "FK_EFTDetails_CashUpDays_DayId",
                        column: x => x.DayId,
                        principalTable: "CashUpDays",
                        principalColumn: "DayId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashierRows_DayId",
                table: "CashierRows",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_CashUpDays_StoreId_CashUpDate",
                table: "CashUpDays",
                columns: new[] { "StoreId", "CashUpDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EFTDetails_DayId",
                table: "EFTDetails",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreCode",
                table: "Stores",
                column: "StoreCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashierRows");

            migrationBuilder.DropTable(
                name: "EFTDetails");

            migrationBuilder.DropTable(
                name: "CashUpDays");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
