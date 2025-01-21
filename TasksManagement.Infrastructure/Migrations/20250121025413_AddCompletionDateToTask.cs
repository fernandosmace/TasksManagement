using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletionDateToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CompletionDate",
                table: "Tasks",
                column: "CompletionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_CompletionDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Tasks");
        }
    }
}
