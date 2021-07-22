using Microsoft.EntityFrameworkCore.Migrations;

namespace iCTF_Shared_Resources.Migrations
{
    public partial class TodaysRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "TodaysRoleId",
                table: "Configuration",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "bca7a057-9cec-4e1b-aec0-7f8afe2524ea");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TodaysRoleId",
                table: "Configuration");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "860d3937-9392-4f06-94ea-3e9ee6eaedd4");
        }
    }
}
