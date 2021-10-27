using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Entities.Migrations
{
    public partial class EntityGroupWithPhotoAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessage_AspNetUsers_SenderId",
                table: "GroupMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessage_Group_GroupId",
                table: "GroupMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_Group_GroupsId",
                table: "GroupUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMessage",
                table: "GroupMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.RenameTable(
                name: "GroupMessage",
                newName: "GroupMessages");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessage_SenderId",
                table: "GroupMessages",
                newName: "IX_GroupMessages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessage_GroupId",
                table: "GroupMessages",
                newName: "IX_GroupMessages_GroupId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Groups",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMessages",
                table: "GroupMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessages_AspNetUsers_SenderId",
                table: "GroupMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessages_Groups_GroupId",
                table: "GroupMessages",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_Groups_GroupsId",
                table: "GroupUser",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessages_AspNetUsers_SenderId",
                table: "GroupMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessages_Groups_GroupId",
                table: "GroupMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_Groups_GroupsId",
                table: "GroupUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMessages",
                table: "GroupMessages");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Groups");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameTable(
                name: "GroupMessages",
                newName: "GroupMessage");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessages_SenderId",
                table: "GroupMessage",
                newName: "IX_GroupMessage_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMessages_GroupId",
                table: "GroupMessage",
                newName: "IX_GroupMessage_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMessage",
                table: "GroupMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessage_AspNetUsers_SenderId",
                table: "GroupMessage",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessage_Group_GroupId",
                table: "GroupMessage",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_Group_GroupsId",
                table: "GroupUser",
                column: "GroupsId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
