using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSample.Migrations
{
    /// <inheritdoc />
    public partial class Version01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PayMaster");

            migrationBuilder.CreateTable(
                name: "PaymentGatewayProvider",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Configurations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PaymentGatewayProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptIssuer",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ShowPaymentResultPage = table.Column<bool>(type: "bit", nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebHookUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_ReceiptIssuer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptRequest",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AdditionalData = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    NationalityCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsLegal = table.Column<bool>(type: "bit", nullable: true),
                    PartyIdentity = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PartyId = table.Column<long>(type: "bigint", nullable: true),
                    IssuerId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ReceiptRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRequest_ReceiptIssuer_IssuerId",
                        column: x => x.IssuerId,
                        principalSchema: "PayMaster",
                        principalTable: "ReceiptIssuer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptRequestGatewayPayment",
                schema: "PayMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusDescription = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FailedReason = table.Column<int>(type: "int", nullable: false),
                    GatewayProviderId = table.Column<long>(type: "bigint", nullable: false),
                    CreateReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RedirectAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CallBackAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SuccessReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceRetrievalNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Pan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TerminalId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    MerchantId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PaymentRequestId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptRequestGatewayPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRequestGatewayPayment_PaymentGatewayProvider_GatewayProviderId",
                        column: x => x.GatewayProviderId,
                        principalSchema: "PayMaster",
                        principalTable: "PaymentGatewayProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptRequestGatewayPayment_ReceiptRequest_PaymentRequestId",
                        column: x => x.PaymentRequestId,
                        principalSchema: "PayMaster",
                        principalTable: "ReceiptRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptRequestTryLog",
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
                    table.PrimaryKey("PK_ReceiptRequestTryLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRequestTryLog_ReceiptRequest_ReceiptRequestId",
                        column: x => x.ReceiptRequestId,
                        principalSchema: "PayMaster",
                        principalTable: "ReceiptRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequest_IssuerId",
                schema: "PayMaster",
                table: "ReceiptRequest",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequestGatewayPayment_GatewayProviderId",
                schema: "PayMaster",
                table: "ReceiptRequestGatewayPayment",
                column: "GatewayProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequestGatewayPayment_PaymentRequestId",
                schema: "PayMaster",
                table: "ReceiptRequestGatewayPayment",
                column: "PaymentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRequestTryLog_ReceiptRequestId",
                schema: "PayMaster",
                table: "ReceiptRequestTryLog",
                column: "ReceiptRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptRequestGatewayPayment",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "ReceiptRequestTryLog",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "PaymentGatewayProvider",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "ReceiptRequest",
                schema: "PayMaster");

            migrationBuilder.DropTable(
                name: "ReceiptIssuer",
                schema: "PayMaster");
        }
    }
}
