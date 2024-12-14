using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class parameters_des_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "patterns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "parameters",
                table: "patterns",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "patterns");

            migrationBuilder.DropColumn(
                name: "parameters",
                table: "patterns");
        }
    }
}
