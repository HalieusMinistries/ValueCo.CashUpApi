using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValueCo.CashUpApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBankAndSpeedPointToStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpeedPointId",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "SpeedPointId",
                table: "Stores");
        }
    }
}
