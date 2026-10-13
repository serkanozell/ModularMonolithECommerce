using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations.Sagas
{
    /// <inheritdoc />
    public partial class OrderSagaAddFailureReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "failed_stock_reservations",
                schema: "ordering",
                table: "order_checkout_saga_states");

            migrationBuilder.DropColumn(
                name: "pending_stock_reservations",
                schema: "ordering",
                table: "order_checkout_saga_states");

            migrationBuilder.AddColumn<string>(
                name: "failure_reason",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "failure_reason",
                schema: "ordering",
                table: "order_checkout_saga_states");

            migrationBuilder.AddColumn<int>(
                name: "failed_stock_reservations",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pending_stock_reservations",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
