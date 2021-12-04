using Microsoft.EntityFrameworkCore.Migrations;

namespace iCTF_Shared_Resources.Migrations
{
    public partial class Foreign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteUsername",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ChallengeTitle",
                table: "Solves");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Solves");

            migrationBuilder.DropColumn(
                name: "Solves",
                table: "Challenges");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.AlterColumn<int>(
                name: "ReleaseTime",
                table: "Configuration",
                type: "int",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "Challenges",
                type: "int",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "Challenges",
                type: "int",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.CreateTable(
                name: "ChallengeUser",
                columns: table => new
                {
                    SolvedChallengesId = table.Column<int>(type: "int", nullable: false),
                    SolvesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeUser", x => new { x.SolvedChallengesId, x.SolvesId });
                    table.ForeignKey(
                        name: "FK_ChallengeUser_Challenges_SolvedChallengesId",
                        column: x => x.SolvedChallengesId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallengeUser_Users_SolvesId",
                        column: x => x.SolvesId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "f71dddd2-54c5-4ca2-8219-b62f63fed226");

            migrationBuilder.CreateIndex(
                name: "IX_Solves_ChallengeId",
                table: "Solves",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Solves_UserId",
                table: "Solves",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserId",
                table: "AspNetUsers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeUser_SolvesId",
                table: "ChallengeUser",
                column: "SolvesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Users_UserId",
                table: "AspNetUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solves_Challenges_ChallengeId",
                table: "Solves",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solves_Users_UserId",
                table: "Solves",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Users_UserId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Solves_Challenges_ChallengeId",
                table: "Solves");

            migrationBuilder.DropForeignKey(
                name: "FK_Solves_Users_UserId",
                table: "Solves");

            migrationBuilder.DropTable(
                name: "ChallengeUser");

            migrationBuilder.DropIndex(
                name: "IX_Solves_ChallengeId",
                table: "Solves");

            migrationBuilder.DropIndex(
                name: "IX_Solves_UserId",
                table: "Solves");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<uint>(
                name: "Score",
                table: "Users",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUsername",
                table: "Users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChallengeTitle",
                table: "Solves",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Solves",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "ReleaseTime",
                table: "Configuration",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<uint>(
                name: "Priority",
                table: "Challenges",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<uint>(
                name: "Points",
                table: "Challenges",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Solves",
                table: "Challenges",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "8797b05b-35a2-4a40-99d0-b5a532d00de7");
        }
    }
}
