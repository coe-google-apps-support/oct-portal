using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class AddApexId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InitiativeId",
                table: "InitiativeStatusHistories",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<int>(
                name: "ApexId",
                table: "Initiatives",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InitiativeStatusHistories_InitiativeId",
                table: "InitiativeStatusHistories",
                column: "InitiativeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InitiativeStatusHistories_Initiatives_InitiativeId",
                table: "InitiativeStatusHistories",
                column: "InitiativeId",
                principalTable: "Initiatives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitiativeStatusHistories_Initiatives_InitiativeId",
                table: "InitiativeStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_InitiativeStatusHistories_InitiativeId",
                table: "InitiativeStatusHistories");

            migrationBuilder.DropColumn(
                name: "ApexId",
                table: "Initiatives");

            migrationBuilder.AlterColumn<Guid>(
                name: "InitiativeId",
                table: "InitiativeStatusHistories",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
