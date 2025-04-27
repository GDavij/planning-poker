using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DropCompositeKeyfromStoryPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryPoints_StoryId_MatchId_AccountId",
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
                name: "IX_StoryPoints_StoryId",
                table: "StoryPoints",
                column: "StoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryPoints",
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
        }
    }
}
