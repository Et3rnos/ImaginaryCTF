using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iCTF_Shared_Resources.Migrations
{
    public partial class TeamSolves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Teams_TeamId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeUser_Users_SolvesId",
                table: "ChallengeUser");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_TeamId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Challenges");

            migrationBuilder.RenameColumn(
                name: "SolvesId",
                table: "ChallengeUser",
                newName: "UserSolvesId");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeUser_SolvesId",
                table: "ChallengeUser",
                newName: "IX_ChallengeUser_UserSolvesId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Users",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Teams",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Configuration",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Challenges",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.CreateTable(
                name: "ChallengeTeam",
                columns: table => new
                {
                    SolvedChallengesId = table.Column<int>(type: "int", nullable: false),
                    TeamSolvesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeTeam", x => new { x.SolvedChallengesId, x.TeamSolvesId });
                    table.ForeignKey(
                        name: "FK_ChallengeTeam_Challenges_SolvedChallengesId",
                        column: x => x.SolvedChallengesId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallengeTeam_Teams_TeamSolvesId",
                        column: x => x.TeamSolvesId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "312d19c0-3697-43f9-bb05-b7cb7010f54a");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeTeam_TeamSolvesId",
                table: "ChallengeTeam",
                column: "TeamSolvesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeUser_Users_UserSolvesId",
                table: "ChallengeUser",
                column: "UserSolvesId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeUser_Users_UserSolvesId",
                table: "ChallengeUser");

            migrationBuilder.DropTable(
                name: "ChallengeTeam");

            migrationBuilder.RenameColumn(
                name: "UserSolvesId",
                table: "ChallengeUser",
                newName: "SolvesId");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeUser_UserSolvesId",
                table: "ChallengeUser",
                newName: "IX_ChallengeUser_SolvesId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Users",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Teams",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Configuration",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "Challenges",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "365af12d-bbee-42de-a35b-63a5a6fdb69e",
                column: "ConcurrencyStamp",
                value: "1a27db5d-ca70-4cf7-8a7c-045846631540");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_TeamId",
                table: "Challenges",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Teams_TeamId",
                table: "Challenges",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeUser_Users_SolvesId",
                table: "ChallengeUser",
                column: "SolvesId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
