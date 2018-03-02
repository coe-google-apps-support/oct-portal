using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class AddingAssigneeToIdeaStatusHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssigneeId",
                table: "IdeaStatusHistories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdeaStatusHistories_AssigneeId",
                table: "IdeaStatusHistories",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaStatusHistories_People_AssigneeId",
                table: "IdeaStatusHistories",
                column: "AssigneeId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaStatusHistories_People_AssigneeId",
                table: "IdeaStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_IdeaStatusHistories_AssigneeId",
                table: "IdeaStatusHistories");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "IdeaStatusHistories");
        }
    }
}
