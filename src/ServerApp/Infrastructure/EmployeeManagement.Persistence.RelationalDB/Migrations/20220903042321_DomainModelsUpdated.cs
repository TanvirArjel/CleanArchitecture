using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagement.Persistence.RelationalDB.Migrations
{
    public partial class DomainModelsUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IdentityKey",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IdentityKey",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Employees",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Departments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Employees",
                newName: "Name");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Employees",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getutcdate()");

            migrationBuilder.AddColumn<int>(
                name: "IdentityKey",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Departments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "IdentityKey",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);
        }
    }
}
