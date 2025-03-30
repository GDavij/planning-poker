using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirebaseUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts_AccountId", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles_RoleId", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    HasStarted = table.Column<bool>(type: "bit", nullable: false),
                    HasClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches_MatchId", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_Matches_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<byte>(type: "tinyint", nullable: false),
                    IsSpectating = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants_AccountId_MatchId", x => new { x.AccountId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_Participants_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participants_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId");
                    table.ForeignKey(
                        name: "FK_Roles_Participants_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    StoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    StoryNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Order = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories_StoryId", x => x.StoryId);
                    table.ForeignKey(
                        name: "FK_Stories_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPoints",
                columns: table => new
                {
                    StoryId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    Points = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPoints_StoryId_MatchId_AccountId", x => new { x.StoryId, x.MatchId, x.AccountId });
                    table.ForeignKey(
                        name: "FK_Participant_StoryPoint_AccountId_StoryId",
                        columns: x => new { x.AccountId, x.StoryId },
                        principalTable: "Participants",
                        principalColumns: new[] { "AccountId", "MatchId" });
                    table.ForeignKey(
                        name: "FK_StoryPoints_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Abbreviation", "Name" },
                values: new object[,]
                {
                    { (byte)1, "PO", "Product Owner" },
                    { (byte)2, "SM", "Scrum Master" },
                    { (byte)3, "Dev", "Software Developer" },
                    { (byte)4, "QA", "Quality Assurance" },
                    { (byte)5, "UX/UI", "UX UI Designer" },
                    { (byte)6, "BA", "Business Analyst" },
                    { (byte)7, "PM", "Project Manager" },
                    { (byte)8, "TL", "Tech Lead" },
                    { (byte)9, "DevOps", "DevOps Engineer" },
                    { (byte)10, "DS", "Data Scientist" },
                    { (byte)11, "MLE", "Machine Learning Engineer" },
                    { (byte)12, "SA", "Solutions Architect" },
                    { (byte)13, "SE", "Software Engineer" },
                    { (byte)14, "DBA", "Database Administrator" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_AccountId",
                table: "Matches",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_MatchId",
                table: "Participants",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_RoleId",
                table: "Participants",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_MatchId",
                table: "Stories",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPoints_AccountId_StoryId",
                table: "StoryPoints",
                columns: new[] { "AccountId", "StoryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryPoints");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
