using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations.Sagas
{
    /// <inheritdoc />
    public partial class AddSomePropertiesToOrderCheckoutSagaState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "completed_at_utc",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "customer_id",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "started_at_utc",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "stock_timeout_token_id",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_amount",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "completed_at_utc",
                schema: "ordering",
                table: "order_checkout_saga_states");

            migrationBuilder.DropColumn(
                name: "customer_id",
                schema: "ordering",
                table: "order_checkout_saga_states");

            migrationBuilder.DropColumn(
                name: "started_at_utc",
                schema: "ordering",
                table: "order_checkout_saga_states");

            migrationBuilder.DropColumn(
                name: "stock_timeout_token_id",
                schema: "ordering",
                table: "order_checkout_saga_states");

            migrationBuilder.DropColumn(
                name: "total_amount",
                schema: "ordering",
                table: "order_checkout_saga_states");
        }
    }
}
