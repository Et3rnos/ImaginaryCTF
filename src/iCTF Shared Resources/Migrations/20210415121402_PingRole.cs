using Microsoft.EntityFrameworkCore.Migrations;

namespace iCTF_Shared_Resources.Migrations
{
    public partial class PingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ChallengePingRole",
                table: "Configuration",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "6b53aede-3d59-4eae-9501-0ef8c8c6587a");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChallengePingRole",
                table: "Configuration");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "0c9ea6b7-b089-45c0-a70c-3d28829934a1");
        }
    }
}
