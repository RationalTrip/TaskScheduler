using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskScheduler.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginAuthId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "LoginAuths",
                columns: table => new
                {
                    AuthId = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAuths", x => x.AuthId);
                    table.ForeignKey(
                        name: "FK_LoginAuths_Users_AuthId",
                        column: x => x.AuthId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskPriority = table.Column<int>(type: "int", nullable: false),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    IsRepetitive = table.Column<bool>(type: "bit", nullable: false),
                    RepetitivePeriod = table.Column<int>(type: "int", nullable: false),
                    RepetitiveEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTasks", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_ScheduleTasks_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participation",
                columns: table => new
                {
                    ParticipantUserIdUserId = table.Column<int>(type: "int", nullable: false),
                    ParticipatedTaskIdTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participation", x => new { x.ParticipantUserIdUserId, x.ParticipatedTaskIdTaskId });
                    table.ForeignKey(
                        name: "FK_Participation_ScheduleTasks_ParticipatedTaskIdTaskId",
                        column: x => x.ParticipatedTaskIdTaskId,
                        principalTable: "ScheduleTasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participation_Users_ParticipantUserIdUserId",
                        column: x => x.ParticipantUserIdUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAuths_Login",
                table: "LoginAuths",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participation_ParticipatedTaskIdTaskId",
                table: "Participation",
                column: "ParticipatedTaskIdTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTasks_Link",
                table: "ScheduleTasks",
                column: "Link",
                unique: true,
                filter: "[Link] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTasks_OwnerUserId",
                table: "ScheduleTasks",
                column: "OwnerUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAuths");

            migrationBuilder.DropTable(
                name: "Participation");

            migrationBuilder.DropTable(
                name: "ScheduleTasks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
