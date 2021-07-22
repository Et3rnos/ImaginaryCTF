using Microsoft.EntityFrameworkCore.Migrations;

namespace iCTF_Shared_Resources.Migrations
{
    public partial class WebUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteUsername",
                table: "Users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "860d3937-9392-4f06-94ea-3e9ee6eaedd4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteUsername",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "7f4cd520-e417-4f4e-9786-df5bd1bbe591");
        }
    }
}
