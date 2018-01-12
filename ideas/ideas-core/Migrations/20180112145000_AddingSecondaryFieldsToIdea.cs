using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class AddingSecondaryFieldsToIdea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "BusinessSponsor",
                table: "Ideas",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Ideas",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedTargetDate",
                table: "Ideas",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBudget",
                table: "Ideas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBusinessSponsor",
                table: "Ideas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Ideas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BusinessSponsor",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "ExpectedTargetDate",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "HasBudget",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "HasBusinessSponsor",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Ideas");
        }
    }
}
