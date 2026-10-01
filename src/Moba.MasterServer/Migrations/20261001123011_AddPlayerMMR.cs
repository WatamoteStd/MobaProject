using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobaServer.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerMMR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MMR",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MMR",
                table: "Users");
        }
    }
}
