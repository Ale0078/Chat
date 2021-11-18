using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Entities.Migrations
{
    public partial class IsMessageRead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "ChatMessages");
        }
    }
}
