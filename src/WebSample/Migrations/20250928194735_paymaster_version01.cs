using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSample.Migrations
{
    /// <inheritdoc />
    public partial class paymaster_version01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PayMaster");

            migrationBuilder.CreateTable(
                name: "PaymentGatewayProfiles",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    JsonConfigurations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedSources = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedSources = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptIssuers",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedSources = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedSources = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptIssuers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptRequests",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IssuerId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AdditionalData = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    NationalityCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsLegal = table.Column<bool>(type: "bit", nullable: true),
                    IssuerReference = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PartyReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PartyId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedSources = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedSources = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRequests_ReceiptIssuers_IssuerId",
                        column: x => x.IssuerId,
                        principalSchema: "PayMaster",
                        principalTable: "ReceiptIssuers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptRequestGatewayPayments",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusDescription = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FailedReason = table.Column<int>(type: "int", nullable: false),
                    PaymentGatewayProfileId = table.Column<long>(type: "bigint", nullable: false),
                    CreateReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RedirectAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CallbackAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CallbackData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuccessReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceRetrievalNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Pan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TerminalId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    MerchantId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ReceiptRequestId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptRequestGatewayPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRequestGatewayPayments_PaymentGatewayProfiles_PaymentGatewayProfileId",
                        column: x => x.PaymentGatewayProfileId,
                        principalSchema: "PayMaster",
                        principalTable: "PaymentGatewayProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptRequestGatewayPayments_ReceiptRequests_ReceiptRequestId",
                        column: x => x.ReceiptRequestId,
                        principalSchema: "PayMaster",
                        principalTable: "ReceiptRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptRequestTryLogs",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptRequestId = table.Column<long>(type: "bigint", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReceiptRequestGatewayPaymentId = table.Column<long>(type: "bigint", nullable: true),
                    TryType = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ExpiredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptRequestTryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRequestTryLogs_ReceiptRequests_ReceiptRequestId",
                        column: x => x.ReceiptRequestId,
                        principalSchema: "PayMaster",
                        principalTable: "ReceiptRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequestGatewayPayments_PaymentGatewayProfileId",
                schema: "PayMaster",
                table: "ReceiptRequestGatewayPayments",
                column: "PaymentGatewayProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequestGatewayPayments_ReceiptRequestId",
                schema: "PayMaster",
                table: "ReceiptRequestGatewayPayments",
                column: "ReceiptRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequests_IssuerId",
                schema: "PayMaster",
                table: "ReceiptRequests",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequestTryLogs_ReceiptRequestId",
                schema: "PayMaster",
                table: "ReceiptRequestTryLogs",
                column: "ReceiptRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptRequestGatewayPayments",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "ReceiptRequestTryLogs",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "PaymentGatewayProfiles",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "ReceiptRequests",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "ReceiptIssuers",
                schema: "PayMaster");
        }
    }
}
