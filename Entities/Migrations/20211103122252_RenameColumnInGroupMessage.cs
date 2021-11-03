using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Entities.Migrations
{
    public partial class RenameColumnInGroupMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEdite",
                table: "GroupMessages",
                newName: "IsEdit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEdit",
                table: "GroupMessages",
                newName: "IsEdite");
        }
    }
}
