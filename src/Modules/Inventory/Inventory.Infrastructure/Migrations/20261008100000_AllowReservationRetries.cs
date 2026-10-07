using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    [DbContext(typeof(InventoryDbContext))]
    [Migration("20261008100000_AllowReservationRetries")]
    public partial class AllowReservationRetries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stock_reservations_inventory_item_id_order_id",
                schema: "inventory",
                table: "stock_reservations");

            migrationBuilder.CreateIndex(
                name: "ix_stock_reservations_inventory_item_id_order_id",
                schema: "inventory",
                table: "stock_reservations",
                columns: new[] { "inventory_item_id", "order_id" },
                unique: true,
                filter: "\"is_active\" = TRUE AND \"is_deleted\" = FALSE");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stock_reservations_inventory_item_id_order_id",
                schema: "inventory",
                table: "stock_reservations");

            migrationBuilder.CreateIndex(
                name: "ix_stock_reservations_inventory_item_id_order_id",
                schema: "inventory",
                table: "stock_reservations",
                columns: new[] { "inventory_item_id", "order_id" },
                unique: true);
        }
    }
}
