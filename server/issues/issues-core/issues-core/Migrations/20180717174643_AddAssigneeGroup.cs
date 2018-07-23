using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Issues.Core.Migrations
{
    public partial class AddAssigneeGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Issues",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "AssigneeGroup",
                table: "Issues",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Urgency",
                table: "Issues",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeGroup",
                table: "Issues");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Issues",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
