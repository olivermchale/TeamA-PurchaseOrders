using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamA.PurchaseOrders.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentInformation",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CardNumber = table.Column<string>(nullable: false),
                    ExpiryDate = table.Column<string>(nullable: false),
                    CVC = table.Column<string>(nullable: false),
                    CardholderName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInformation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseStatus",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseStatus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    PurchasedBy = table.Column<Guid>(nullable: false),
                    ProductID = table.Column<Guid>(nullable: false),
                    PurchasedOn = table.Column<DateTime>(nullable: false),
                    StatusID = table.Column<Guid>(nullable: false),
                    PaymentInformationID = table.Column<Guid>(nullable: false),
                    ProductName = table.Column<string>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_PaymentInformation_PaymentInformationID",
                        column: x => x.PaymentInformationID,
                        principalTable: "PaymentInformation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_PurchaseStatus_StatusID",
                        column: x => x.StatusID,
                        principalTable: "PurchaseStatus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PaymentInformationID",
                table: "PurchaseOrders",
                column: "PaymentInformationID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_StatusID",
                table: "PurchaseOrders",
                column: "StatusID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "PaymentInformation");

            migrationBuilder.DropTable(
                name: "PurchaseStatus");
        }
    }
}
