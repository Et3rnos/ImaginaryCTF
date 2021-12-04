using Microsoft.EntityFrameworkCore.Migrations;

namespace iCTF_Shared_Resources.Migrations
{
    public partial class TopRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "FirstPlaceRoleId",
                table: "Configuration",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "SecondPlaceRoleId",
                table: "Configuration",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "ThirdPlaceRoleId",
                table: "Configuration",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "TodaysChannelId",
                table: "Configuration",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "7f4cd520-e417-4f4e-9786-df5bd1bbe591");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPlaceRoleId",
                table: "Configuration");

            migrationBuilder.DropColumn(
                name: "SecondPlaceRoleId",
                table: "Configuration");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceRoleId",
                table: "Configuration");

            migrationBuilder.DropColumn(
                name: "TodaysChannelId",
                table: "Configuration");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "ce3d1bd3-06d4-4165-9197-c35bfeb54f36");
        }
    }
}
