using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaiTh.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponIdToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CouponId",
                table: "Products",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Coupons_CouponId",
                table: "Products",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Coupons_CouponId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CouponId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Products");
        }
    }
}
