using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ReturnCompositePrimaryKeyWithCorrectParticipantMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_StoryPoint_AccountId_StoryId",
                table: "StoryPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryPoints",
                table: "StoryPoints");

            migrationBuilder.DropIndex(
                name: "IX_StoryPoints_AccountId_StoryId",
                table: "StoryPoints");

            migrationBuilder.DropIndex(
                name: "IX_StoryPoints_StoryId",
                table: "StoryPoints");

            migrationBuilder.DropColumn(
                name: "StoryPointId",
                table: "StoryPoints");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryPoints_StoryId_MatchId_AccountId",
                table: "StoryPoints",
                columns: new[] { "StoryId", "MatchId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryPoints_AccountId_MatchId",
                table: "StoryPoints",
                columns: new[] { "AccountId", "MatchId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_StoryPoint_AccountId_StoryId",
                table: "StoryPoints",
                columns: new[] { "AccountId", "MatchId" },
                principalTable: "Participants",
                principalColumns: new[] { "AccountId", "MatchId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_StoryPoint_AccountId_StoryId",
                table: "StoryPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryPoints_StoryId_MatchId_AccountId",
                table: "StoryPoints");

            migrationBuilder.DropIndex(
                name: "IX_StoryPoints_AccountId_MatchId",
                table: "StoryPoints");

            migrationBuilder.AddColumn<long>(
                name: "StoryPointId",
                table: "StoryPoints",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryPoints",
                table: "StoryPoints",
                column: "StoryPointId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPoints_AccountId_StoryId",
                table: "StoryPoints",
                columns: new[] { "AccountId", "StoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryPoints_StoryId",
                table: "StoryPoints",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_StoryPoint_AccountId_StoryId",
                table: "StoryPoints",
                columns: new[] { "AccountId", "StoryId" },
                principalTable: "Participants",
                principalColumns: new[] { "AccountId", "MatchId" });
        }
    }
}
