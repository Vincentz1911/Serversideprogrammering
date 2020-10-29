using Microsoft.EntityFrameworkCore.Migrations;

namespace Serversideprogrammering.Migrations
{
    public partial class addedview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoneBefore",
                table: "TodoItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoneBefore",
                table: "TodoItem");
        }
    }
}
