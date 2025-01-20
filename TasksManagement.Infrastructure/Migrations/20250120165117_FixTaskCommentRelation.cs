using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTaskCommentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks_TaskItemId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TaskItemId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TaskItemId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskItemId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TaskItemId",
                table: "Comments",
                column: "TaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks_TaskItemId",
                table: "Comments",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
