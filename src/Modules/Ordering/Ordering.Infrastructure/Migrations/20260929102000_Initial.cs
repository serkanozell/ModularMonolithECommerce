using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ordering");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "ordering",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    billing_address_address_line = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    billing_address_country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    billing_address_email_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    billing_address_first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    billing_address_last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    billing_address_state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    billing_address_zip_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    payment_cvv = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    payment_card_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_card_number = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    payment_expiration = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    payment_payment_method = table.Column<int>(type: "integer", nullable: false),
                    shipping_address_address_line = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    shipping_address_country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    shipping_address_email_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    shipping_address_first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    shipping_address_last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    shipping_address_state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    shipping_address_zip_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "ordering",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "ordering",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_order_items_order_id",
                schema: "ordering",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_id",
                schema: "ordering",
                table: "orders",
                column: "customer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_items",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "ordering");
        }
    }
}
