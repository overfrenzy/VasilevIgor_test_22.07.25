using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VasilevIgor_test_22._07._25.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderLastNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_employees_Gender_LastName",
                table: "employees",
                columns: new[] { "Gender", "LastName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employees_Gender_LastName",
                table: "employees");
        }
    }
}
