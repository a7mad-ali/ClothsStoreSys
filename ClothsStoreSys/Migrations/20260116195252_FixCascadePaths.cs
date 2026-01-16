using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothsStoreSys.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnItems_InvoiceItems_InvoiceItemId",
                table: "ReturnItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Invoices_InvoiceId",
                table: "Returns");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnItems_InvoiceItems_InvoiceItemId",
                table: "ReturnItems",
                column: "InvoiceItemId",
                principalTable: "InvoiceItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Invoices_InvoiceId",
                table: "Returns",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnItems_InvoiceItems_InvoiceItemId",
                table: "ReturnItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Invoices_InvoiceId",
                table: "Returns");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnItems_InvoiceItems_InvoiceItemId",
                table: "ReturnItems",
                column: "InvoiceItemId",
                principalTable: "InvoiceItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Invoices_InvoiceId",
                table: "Returns",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
