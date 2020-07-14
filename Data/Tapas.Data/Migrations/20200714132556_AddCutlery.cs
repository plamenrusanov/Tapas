namespace Tapas.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddCutlery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cutlery",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "DeliveryTaxes",
                type: "decimal(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cutlery",
                table: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "DeliveryTaxes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 2)");
        }
    }
}
