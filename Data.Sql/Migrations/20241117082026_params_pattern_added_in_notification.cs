using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class params_pattern_added_in_notification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "params",
                table: "notifications",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "pattern_id",
                table: "notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_pattern_id",
                table: "notifications",
                column: "pattern_id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_patterns_pattern_id",
                table: "notifications",
                column: "pattern_id",
                principalTable: "patterns",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_patterns_pattern_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_pattern_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "params",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "pattern_id",
                table: "notifications");
        }
    }
}
