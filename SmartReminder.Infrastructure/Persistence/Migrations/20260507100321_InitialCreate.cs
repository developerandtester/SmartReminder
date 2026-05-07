using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartReminder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationParticipants_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentParentLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CanViewProgress = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateTasks = table.Column<bool>(type: "bit", nullable: false),
                    CanReceiveCriticalAlerts = table.Column<bool>(type: "bit", nullable: false),
                    CanViewVisualSchedule = table.Column<bool>(type: "bit", nullable: false),
                    CanViewMoodNotes = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentParentLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentParentLinks_Users_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentParentLinks_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentTeacherLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CanViewProgress = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateTasks = table.Column<bool>(type: "bit", nullable: false),
                    CanViewVisualSchedule = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTeacherLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentTeacherLinks_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTeacherLinks_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisualSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    ScheduleDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisualSchedules_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisualSchedules_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    SenderUserId = table.Column<int>(type: "int", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    IsFromLlm = table.Column<bool>(type: "bit", nullable: false),
                    CanBeConvertedToTask = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTaskId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReminderTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MissedCount = table.Column<int>(type: "int", nullable: false),
                    IsSuggestedByParentOrTeacher = table.Column<bool>(type: "bit", nullable: false),
                    RequiresStudentApproval = table.Column<bool>(type: "bit", nullable: false),
                    SourceChatMessageId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReminderTasks_ChatMessages_SourceChatMessageId",
                        column: x => x.SourceChatMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReminderTasks_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReminderTasks_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PomodoroSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReminderTaskId = table.Column<int>(type: "int", nullable: true),
                    FocusDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    ShortBreakMinutes = table.Column<int>(type: "int", nullable: false),
                    LongBreakMinutes = table.Column<int>(type: "int", nullable: false),
                    CycleNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PausedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PomodoroSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PomodoroSessions_ReminderTasks_ReminderTaskId",
                        column: x => x.ReminderTaskId,
                        principalTable: "ReminderTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PomodoroSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReminderTaskId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSteps_ReminderTasks_ReminderTaskId",
                        column: x => x.ReminderTaskId,
                        principalTable: "ReminderTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisualScheduleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisualScheduleId = table.Column<int>(type: "int", nullable: false),
                    LinkedTaskId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisualScheduleItems_ReminderTasks_LinkedTaskId",
                        column: x => x.LinkedTaskId,
                        principalTable: "ReminderTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VisualScheduleItems_VisualSchedules_VisualScheduleId",
                        column: x => x.VisualScheduleId,
                        principalTable: "VisualSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ConversationId",
                table: "ChatMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_CreatedTaskId",
                table: "ChatMessages",
                column: "CreatedTaskId",
                unique: true,
                filter: "[CreatedTaskId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderUserId",
                table: "ChatMessages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationId_UserId",
                table: "ConversationParticipants",
                columns: new[] { "ConversationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_UserId",
                table: "ConversationParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PomodoroSessions_ReminderTaskId",
                table: "PomodoroSessions",
                column: "ReminderTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PomodoroSessions_UserId",
                table: "PomodoroSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderTasks_CreatedByUserId",
                table: "ReminderTasks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderTasks_OwnerUserId",
                table: "ReminderTasks",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderTasks_SourceChatMessageId",
                table: "ReminderTasks",
                column: "SourceChatMessageId",
                unique: true,
                filter: "[SourceChatMessageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudentParentLinks_ParentId",
                table: "StudentParentLinks",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentParentLinks_StudentId_ParentId",
                table: "StudentParentLinks",
                columns: new[] { "StudentId", "ParentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTeacherLinks_StudentId_TeacherId",
                table: "StudentTeacherLinks",
                columns: new[] { "StudentId", "TeacherId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTeacherLinks_TeacherId",
                table: "StudentTeacherLinks",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSteps_ReminderTaskId",
                table: "TaskSteps",
                column: "ReminderTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisualScheduleItems_LinkedTaskId",
                table: "VisualScheduleItems",
                column: "LinkedTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualScheduleItems_VisualScheduleId",
                table: "VisualScheduleItems",
                column: "VisualScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualSchedules_CreatedByUserId",
                table: "VisualSchedules",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualSchedules_StudentId_ScheduleDate",
                table: "VisualSchedules",
                columns: new[] { "StudentId", "ScheduleDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ReminderTasks_CreatedTaskId",
                table: "ChatMessages",
                column: "CreatedTaskId",
                principalTable: "ReminderTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ReminderTasks_CreatedTaskId",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ConversationParticipants");

            migrationBuilder.DropTable(
                name: "PomodoroSessions");

            migrationBuilder.DropTable(
                name: "StudentParentLinks");

            migrationBuilder.DropTable(
                name: "StudentTeacherLinks");

            migrationBuilder.DropTable(
                name: "TaskSteps");

            migrationBuilder.DropTable(
                name: "VisualScheduleItems");

            migrationBuilder.DropTable(
                name: "VisualSchedules");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "ReminderTasks");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
