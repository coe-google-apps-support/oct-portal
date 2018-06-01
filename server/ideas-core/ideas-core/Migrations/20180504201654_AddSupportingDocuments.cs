using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class AddSupportingDocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.CreateTable(
                name: "SupportingDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InitiativeId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    URL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportingDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportingDocuments_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportingDocuments_InitiativeId",
                table: "SupportingDocuments",
                column: "InitiativeId");
            



            

            migrationBuilder.Sql(@"
INSERT INTO SupportingDocuments(InitiativeId, Type, URL) SELECT Id, Type = 1, BusinessCaseUrl FROM Initiatives WHERE BusinessCaseUrl IS NOT NULL;
INSERT INTO SupportingDocuments(InitiativeId, Type, URL) SELECT Id, Type = 2, InvestmentRequestFormUrl FROM Initiatives WHERE InvestmentRequestFormUrl IS NOT NULL; "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM SupportingDocuments WHERE Type = 1; 
DELETE FROM SupportingDocuments WHERE Type = 2;");

            migrationBuilder.DropTable(
                name: "SupportingDocuments");
        }
    }
}
