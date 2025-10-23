using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Permission",
                table: "Users",
                type: "VARCHAR(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "DECIMAL(7,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(65,30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Permission",
                table: "Users",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(10)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "DECIMAL(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(7,4)");
        }
    }
}
