using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Issues.Core.Migrations
{
    public partial class UpdateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssigneeEmail",
                table: "Issues",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Issues",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemedyStatus",
                table: "Issues",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestorName",
                table: "Issues",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeEmail",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "RemedyStatus",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "RequestorName",
                table: "Issues");
        }
    }
}
