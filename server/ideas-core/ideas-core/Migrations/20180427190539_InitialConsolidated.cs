using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class InitialConsolidated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Initiatives",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApexId = table.Column<int>(nullable: true),
                    AssigneeId = table.Column<int>(nullable: true),
                    BusinessCaseUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    InvestmentRequestFormUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Uid = table.Column<Guid>(nullable: false),
                    WorkOrderId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Initiatives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusEtas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EtaType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEtas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StringTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 64, nullable: true),
                    Text = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InitiativeStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpectedExitDateUtc = table.Column<DateTime>(nullable: true),
                    InitiativeId = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusDescriptionOverride = table.Column<string>(nullable: true),
                    StatusEntryDateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitiativeStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitiativeStatusHistories_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stakeholder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InitiativeId = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stakeholder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stakeholder_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_WorkOrderId",
                table: "Initiatives",
                column: "WorkOrderId",
                unique: true,
                filter: "[WorkOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InitiativeStatusHistories_InitiativeId",
                table: "InitiativeStatusHistories",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Stakeholder_InitiativeId",
                table: "Stakeholder",
                column: "InitiativeId");

            migrationBuilder.Sql("INSERT INTO [dbo].[StatusEtas] ([EtaType],[Status],[Time]) VALUES(" +
                (int)Data.EtaType.BusinessSeconds + "," +
                (int)Data.InitiativeStatus.Submit + ",14400)");

            migrationBuilder.Sql(@"
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Submit_present', 'Thank you! Your initiative has been submitted and will be assigned for review. An OCT representative will contact you by {0}')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Submit_past', 'Thank you! Your initiative was submitted.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Review_present', 'Your initiative has been assigned for review. {0} will be working with you to further define your initiative.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Review_past', 'Your initiative has been assigned and reviewed.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Collaborate_present', '{0} is actively working with you to complete an Investment Request for your initiative so that it may be prioritized and approved.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Collaborate_past', 'An Investment Request has been completed for your initiative.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Deliver_present', 'Your request has been submitted to Technology Investment for Business Technology Steering Committee Approval. A Technology Investment representative will notify you when your initiative has been approved.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Deliver_past', 'Your initiative has been successfully submitted as a project with Solutions Delivery.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Cancelled_present', 'Your request has been cancelled. ')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Cancelled_past', 'Your request was cancelled')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM StringTemplates WHERE Category = 1 AND [Key] IN ('Cancelled_past', 'Cancelled_present', 'Submit_past', 'Submit_present', 'Review_past', 'Review_present', 'Collaborate_past', 'Collaborate_present', 'Deliver_past', 'Deliver_present');");


            migrationBuilder.DropTable(
                name: "InitiativeStatusHistories");

            migrationBuilder.DropTable(
                name: "Stakeholder");

            migrationBuilder.DropTable(
                name: "StatusEtas");

            migrationBuilder.DropTable(
                name: "StringTemplates");

            migrationBuilder.DropTable(
                name: "Initiatives");
        }
    }
}
