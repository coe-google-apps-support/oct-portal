using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class RemoveBusinessCaseandTechnologyInvestment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessCaseUrl",
                table: "Initiatives");

            migrationBuilder.DropColumn(
                name: "InvestmentRequestFormUrl",
                table: "Initiatives");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessCaseUrl",
                table: "Initiatives",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvestmentRequestFormUrl",
                table: "Initiatives",
                maxLength: 2048,
                nullable: true);
        }
    }
}
