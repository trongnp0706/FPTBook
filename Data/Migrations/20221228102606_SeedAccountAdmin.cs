using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTBook.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccountAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "AdminID123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4e064861-77a8-4dd8-b31f-ad887fd96983", "AQAAAAIAAYagAAAAEPdQ99VuNPR9lQEBqlYQupaGz9C+KpxTA9TtTOUyhhz8xDmxbPF8lxb/7knxTPrlQg==", "5ddfcc86-65b3-472c-a0cf-0e9d5757bfff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "AdminID123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1c8affe8-695f-4f4f-86b7-339dd634edbf", "AQAAAAIAAYagAAAAEG3ErSVX1pxqkSdt0f3xe9CkJknshFMp/7kP2fH9XLvUjmKkv7ZoY40o5mNyqEmF6Q==", "cff57885-e87c-402a-ae4f-188039a01f8d" });
        }
    }
}
