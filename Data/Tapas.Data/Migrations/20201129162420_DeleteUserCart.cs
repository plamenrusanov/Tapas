using Microsoft.EntityFrameworkCore.Migrations;

namespace Tapas.Data.Migrations
{
    public partial class DeleteUserCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ShopingCarts_ShopingCartId1",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddresses_AspNetUsers_ApplicationUserId",
                table: "DeliveryAddresses");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryAddresses_ApplicationUserId",
                table: "DeliveryAddresses");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ShopingCartId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShopingCartId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShopingCartId1",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "DeliveryAddresses",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Categories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "DeliveryAddresses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopingCartId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopingCartId1",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddresses_ApplicationUserId",
                table: "DeliveryAddresses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ShopingCartId1",
                table: "AspNetUsers",
                column: "ShopingCartId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ShopingCarts_ShopingCartId1",
                table: "AspNetUsers",
                column: "ShopingCartId1",
                principalTable: "ShopingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddresses_AspNetUsers_ApplicationUserId",
                table: "DeliveryAddresses",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
