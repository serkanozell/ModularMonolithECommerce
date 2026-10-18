using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations.Sagas
{
    /// <inheritdoc />
    public partial class AddNewStatesToOrderingSaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "saga_timeout_token_id",
                schema: "ordering",
                table: "order_checkout_saga_states",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "saga_timeout_token_id",
                schema: "ordering",
                table: "order_checkout_saga_states");
        }
    }
}
