using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Persistence.RelationalDB.Migrations
{
    public partial class IdentityColumnsDropeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdentityKey",
                table: "AspNetRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdentityKey",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "IdentityKey",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
