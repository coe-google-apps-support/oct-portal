using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Issues.Core.Migrations
{
    public partial class StatusUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Issues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Stakeholder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IssueId = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stakeholder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stakeholder_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stakeholder_IssueId",
                table: "Stakeholder",
                column: "IssueId");

            migrationBuilder.Sql(@"
INSERT INTO issues.Issues(`Id`, `AssigneeId`, `CreatedDate`, `Description`, `Title`, `Uid`, `Status`) VALUES(1,2,'2018-07-01', 'Emals always neverr send.', 'How do I sen an email?', 'c6859cb5-3cbe-4483-8b7c-9b4022fa796a', 3);
INSERT INTO issues.Issues(`Id`, `AssigneeId`, `CreatedDate`, `Description`, `Title`, `Uid`, `Status`) VALUES(2,3,'2018-07-03', 'My laptop is broken and I cant do anything!!', 'Broken laptop', 'c6859cb6-3cbe-4483-8b7c-9b4022fa796a', 1);
INSERT INTO issues.Issues(`Id`, `AssigneeId`, `CreatedDate`, `Description`, `Title`, `Uid`, `Status`) VALUES(3,3,'2018-07-03', 'My dev VM is suuuuupoer slow. Please fix it for me.', 'I need a new VM', 'c6859cb7-3cbe-4483-8b7c-9b4022fa796a', 0);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stakeholder");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Issues");
        }
    }
}
