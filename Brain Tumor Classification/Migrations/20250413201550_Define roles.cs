using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Brain_Tumor_Classification.Migrations
{
    /// <inheritdoc />
    public partial class Defineroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6e7aa1d4-bd0d-41a9-a55f-ab2006fc30a8", null, "User", "USER" },
                    { "ca81f9a9-1f35-4c17-b08b-7f06318a87e6", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6e7aa1d4-bd0d-41a9-a55f-ab2006fc30a8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca81f9a9-1f35-4c17-b08b-7f06318a87e6");
        }
    }
}
