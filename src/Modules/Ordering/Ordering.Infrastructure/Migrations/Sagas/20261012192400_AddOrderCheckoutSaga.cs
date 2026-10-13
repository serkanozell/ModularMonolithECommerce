using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations.Sagas
{
    /// <inheritdoc />
    public partial class AddOrderCheckoutSaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ordering");

            migrationBuilder.CreateTable(
                name: "order_checkout_saga_states",
                schema: "ordering",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_state = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pending_stock_reservations = table.Column<int>(type: "integer", nullable: false),
                    failed_stock_reservations = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_checkout_saga_states", x => x.correlation_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_checkout_saga_states",
                schema: "ordering");
        }
    }
}
