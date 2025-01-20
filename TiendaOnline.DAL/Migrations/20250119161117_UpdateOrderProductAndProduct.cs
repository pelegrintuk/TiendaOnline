using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaOnline.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderProductAndProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderProductId",
                table: "OrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderProductId",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderProducts");
        }
    }
}
