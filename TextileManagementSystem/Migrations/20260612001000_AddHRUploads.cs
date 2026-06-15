using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TextileManagementSystem.Data;

#nullable disable

namespace TextileManagementSystem.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260612001000_AddHRUploads")]
    public partial class AddHRUploads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Employees",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportingDocumentPath",
                table: "SalaryAdvances",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SupportingDocumentPath",
                table: "SalaryAdvances");
        }
    }
}
