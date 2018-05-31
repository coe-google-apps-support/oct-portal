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

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(1, 1, 'www.testBusinessCaseUrl.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description1', 'www.googleInvestmentRequestFormUrl.com', 2, 'test title1', '27E22CB9-4C6E-4CF3-8D3F-318BAB0C393A', '1')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(2, 2, 'www.testBusinessCaseUrl.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description2', 'www.googleInvestmentRequestFormUrl.com', 2, 'test title2', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '2')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(3, 3, 'www.testBusinessCaseUrl.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description3', Null, 2, 'test title2', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '3')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(4, 4, 'www.testBusinessCaseUrl.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description4', 'www.googleInvestmentRequestFormUrl.com', 2, 'test title4', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '4')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(5, 5, Null, '2018-05-10 22:24:19.5789984 +00:00', 'test description5', 'www.googleInvestmentRequestFormUrl.com', 2, 'test title5', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '5')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(6, 6, 'www.testBusinessCaseUrl.com', '2018-05-10 22:24:19.5789984 +00:00', 'test description6', 'www.googleInvestmentRequestFormUrl.com', 2, 'test title6', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '6')


			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(5, 5, Null, '2018-05-10 22:24:19.5789984 +00:00', 'test description7', Null, 2, 'test title7', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '7')

			INSERT INTO Initiatives(ApexId, AssigneeId, [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], Status, [Title], [Uid], [WorkOrderId]) VALUES(5, 5, Null, '2018-05-10 22:24:19.5789984 +00:00', 'test description8', Null, 2, 'test title8', '27E22CB9-4C6E-4CF3-8D3F-398BAB0C393A', '8')


			INSERT INTO SupportingDocuments(InitiativeId, Type, URL) SELECT Id, Type = 1, BusinessCaseUrl FROM Initiatives WHERE BusinessCaseUrl IS NOT NULL;

			INSERT INTO SupportingDocuments(InitiativeId, Type, URL) SELECT Id, Type = 2, InvestmentRequestFormUrl FROM Initiatives WHERE InvestmentRequestFormUrl IS NOT NULL; "
            );


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
