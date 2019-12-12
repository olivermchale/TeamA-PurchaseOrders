using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamA.PurchaseOrders.Data.Migrations
{
    public partial class sourceupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentInformation",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CardNumber = table.Column<string>(nullable: false),
                    CardExpiry = table.Column<DateTime>(nullable: false),
                    CardCVC = table.Column<string>(nullable: false),
                    CardName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInformation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    PurchasedBy = table.Column<Guid>(nullable: false),
                    ProductID = table.Column<int>(nullable: false),
                    PurchasedOn = table.Column<DateTime>(nullable: false),
                    StatusID = table.Column<Guid>(nullable: false),
                    PaymentInformationID = table.Column<Guid>(nullable: false),
                    ExternalID = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ProductPrice = table.Column<double>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Postcode = table.Column<string>(nullable: false),
                    Source = table.Column<string>(nullable: true),
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
                        principalColumn: "Id",
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
