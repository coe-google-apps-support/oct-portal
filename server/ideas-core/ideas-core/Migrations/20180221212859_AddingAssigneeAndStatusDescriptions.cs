using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class AddingAssigneeAndStatusDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "Stakeholders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AssigneeId",
                table: "Ideas",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO People(Email, UserName) SELECT Email, UserName FROM Stakeholders;");
            migrationBuilder.Sql("UPDATE Stakeholders S LEFT JOIN (SELECT Id as NewPersonId, Email, UserName FROM People) P ON S.Email = P.Email AND S.UserName = P.UserName SET PersonId = NewPersonId;");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Stakeholders");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Stakeholders");

            migrationBuilder.CreateTable(
                name: "StringTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stakeholders_PersonId",
                table: "Stakeholders",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_AssigneeId",
                table: "Ideas",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_People_AssigneeId",
                table: "Ideas",
                column: "AssigneeId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stakeholders_People_PersonId",
                table: "Stakeholders",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_People_AssigneeId",
                table: "Ideas");

            migrationBuilder.DropForeignKey(
                name: "FK_Stakeholders_People_PersonId",
                table: "Stakeholders");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "StringTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Stakeholders_PersonId",
                table: "Stakeholders");

            migrationBuilder.DropIndex(
                name: "IX_Ideas_AssigneeId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Stakeholders");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Ideas");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Stakeholders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Stakeholders",
                nullable: false,
                defaultValue: "");
        }
    }
}
