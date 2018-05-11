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

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(1, 1, 'www.test.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description', 'www.google.com', 2, 'test title', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '1')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(2, 2, 'www.test.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description', 'www.google.com', 2, 'test title', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '2')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(3, 3, 'www.test.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description', 'www.google.com', 2, 'test title', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '3')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(4, 4, 'www.test.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description', 'www.google.com', 2, 'test title', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '4')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(5, 5, 'www.test.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description', 'www.google.com', 2, 'test title', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '5')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(6, 6, 'www.test.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description', 'www.google.com', 2, 'test title', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '6')");


		}


		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportingDocuments");
        }
    }
}
