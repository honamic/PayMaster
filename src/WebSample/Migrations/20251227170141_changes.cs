using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSample.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedSources",
                schema: "PayMaster",
                table: "ReceiptRequests");

            migrationBuilder.DropColumn(
                name: "ModifiedSources",
                schema: "PayMaster",
                table: "ReceiptRequests");

            migrationBuilder.DropColumn(
                name: "CreatedSources",
                schema: "PayMaster",
                table: "ReceiptIssuers");

            migrationBuilder.DropColumn(
                name: "ModifiedSources",
                schema: "PayMaster",
                table: "ReceiptIssuers");

            migrationBuilder.DropColumn(
                name: "CreatedSources",
                schema: "PayMaster",
                table: "PaymentGatewayProfiles");

            migrationBuilder.DropColumn(
                name: "ModifiedSources",
                schema: "PayMaster",
                table: "PaymentGatewayProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedSources",
                schema: "PayMaster",
                table: "ReceiptRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedSources",
                schema: "PayMaster",
                table: "ReceiptRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedSources",
                schema: "PayMaster",
                table: "ReceiptIssuers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedSources",
                schema: "PayMaster",
                table: "ReceiptIssuers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedSources",
                schema: "PayMaster",
                table: "PaymentGatewayProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedSources",
                schema: "PayMaster",
                table: "PaymentGatewayProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
